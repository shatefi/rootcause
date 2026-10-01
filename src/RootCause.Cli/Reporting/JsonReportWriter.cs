using System.Text.Json;
using System.Text.Json.Serialization;

using RootCause.Advisor;

namespace RootCause.Cli;

public class JsonReportWriter : IReportWriter
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public Task<string> WriteAsync(IEnumerable<Finding> findings, CancellationToken cancellationToken)
    {
        var report = findings.Select(FindingReport.From).ToList();
        return Task.FromResult(JsonSerializer.Serialize(report, SerializerOptions));
    }
}
