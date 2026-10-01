using RootCause.Advisor;

namespace RootCause.Cli;

/// <summary>
/// What a user sees about one finding. Internal fields of <see cref="Finding"/>, such as the rule
/// code and the plan node ID, stay out of the report, and a field added to <see cref="Finding"/>
/// appears here only when it's added deliberately.
/// </summary>
internal sealed record FindingReport(
    Severity Severity,
    string What,
    string? ObjectName,
    string Fix,
    string? FixSql,
    string Cost,
    string SkipWhen)
{
    public static FindingReport From(Finding finding) => new(
        finding.Severity,
        finding.What,
        finding.ObjectName,
        finding.Fix,
        finding.FixSql,
        finding.Cost,
        finding.SkipWhen);
}
