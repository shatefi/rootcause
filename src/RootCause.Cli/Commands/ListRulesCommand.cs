using System.CommandLine;

using Microsoft.Extensions.DependencyInjection;

using RootCause.Advisor.SqlServer;

namespace RootCause.Cli;

public sealed class ListRulesCommand : Command
{
    private readonly IServiceProvider _services;

    public ListRulesCommand(IServiceProvider services) : base("list", "List the rules RootCause checks.")
    {
        _services = services;
        SetAction(ExecuteAsync);
    }

    private async Task<int> ExecuteAsync(ParseResult parseResult, CancellationToken cancellationToken)
    {
        var codes = _services.GetServices<IRule>()
            .Select(rule => rule.Code)
            .Order(StringComparer.Ordinal);

        foreach (var code in codes)
        {
            await parseResult.InvocationConfiguration.Output.WriteLineAsync(code);
        }

        return ExitCodes.Success;
    }
}
