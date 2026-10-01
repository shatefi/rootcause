using System.CommandLine;
using System.Xml;
using System.Xml.Linq;

using Microsoft.Extensions.DependencyInjection;

using RootCause.Advisor;
using RootCause.Advisor.SqlServer;

namespace RootCause.Cli;

public sealed class AnalyzeCommand : Command
{
    private readonly IServiceProvider _services;

    private readonly Option<FileInfo> _fileOption = new("--plan", ["-p"])
    {
        Description = "The execution plan to analyze (.sqlplan or .xml)",
        Required = true
    };

    private readonly Option<ReportFormat> _reportOption = new("--format")
    {
        Description = "How to write the report: text for people, json for scripts",
        DefaultValueFactory = _ => ReportFormat.Text
    };

    private readonly Option<FailOn> _failOnOption = new("--fail-on")
    {
        Description = "The lowest severity that makes the command return exit code 1",
        DefaultValueFactory = _ => FailOn.Critical
    };

    public AnalyzeCommand(IServiceProvider services) : base("analyze", "Analyze an execution plan file.")
    {
        _services = services;

        _fileOption.AcceptExistingOnly();
        _fileOption.Validators.Add(result =>
        {
            var path = result.Tokens.SingleOrDefault()?.Value;
            if (path is null)
            {
                return;
            }

            var extension = Path.GetExtension(path);
            if (!Enum.GetNames<PlanFormat>().Any(n => n.Equals(extension.Trim('.'), StringComparison.OrdinalIgnoreCase)))
            {
                result.AddError($"Unsupported file type '{extension}'. Use a .sqlplan or .xml file.");
            }
        });

        Options.Add(_fileOption);
        Options.Add(_reportOption);
        Options.Add(_failOnOption);

        SetAction(ExecuteAsync);
    }

    private async Task<int> ExecuteAsync(ParseResult parseResult, CancellationToken cancellationToken)
    {
        var file = parseResult.GetRequiredValue(_fileOption);
        var format = parseResult.GetValue(_reportOption);
        var failOn = parseResult.GetValue(_failOnOption);

        IReadOnlyList<Finding> findings;
        try
        {
            await using var stream = File.OpenRead(file.FullName);
            var xml = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);
            findings = _services.GetRequiredService<Analyzer>().Analyze(new PlanDocument(xml));
        }
        catch (Exception ex) when (ex is XmlException or InvalidOperationException)
        {
            await parseResult.InvocationConfiguration.Error.WriteLineAsync($"RootCause: {ex.Message}");
            return ExitCodes.Error;
        }

        var writer = _services.GetRequiredKeyedService<IReportWriter>(format);
        var report = await writer.WriteAsync(findings, cancellationToken);
        await parseResult.InvocationConfiguration.Output.WriteLineAsync(report);

        return findings.Any(finding => finding.Severity >= Threshold(failOn))
            ? ExitCodes.Findings
            : ExitCodes.Success;
    }

    private static Severity Threshold(FailOn failOn) => failOn switch
    {
        FailOn.Warning => Severity.Warning,
        FailOn.Critical => Severity.Critical,
        _ => throw new ArgumentOutOfRangeException(nameof(failOn), failOn, null)
    };
}
