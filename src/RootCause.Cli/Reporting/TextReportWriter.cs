using System.Text;

using RootCause.Advisor;

namespace RootCause.Cli;

public class TextReportWriter : IReportWriter
{
    public Task<string> WriteAsync(IEnumerable<Finding> findings, CancellationToken cancellationToken)
    {
        var report = new StringBuilder();

        foreach (var finding in findings)
        {
            report.AppendLine($"[{finding.Severity}] {finding.What}");

            if (finding.ObjectName is not null)
            {
                report.AppendLine($"  Object:     {finding.ObjectName}");
            }

            report.AppendLine($"  Fix:        {finding.Fix}");

            if (finding.FixSql is not null)
            {
                report.AppendLine($"  Fix SQL:    {finding.FixSql}");
            }

            report.AppendLine($"  Cost:       {finding.Cost}");
            report.AppendLine($"  Skip when:  {finding.SkipWhen}");
            report.AppendLine();
        }

        string text = report.Length == 0 ? "No problems found." : report.ToString().TrimEnd();
        return Task.FromResult(text);
    }
}
