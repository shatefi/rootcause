using RootCause.Advisor;

namespace RootCause.Cli;

public interface IReportWriter
{
    public Task<string> WriteAsync(IEnumerable<Finding> findings, CancellationToken cancellationToken);
}
