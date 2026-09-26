
namespace RootCause.Advisor;

public sealed record class Finding(
    string Code, // "PLAN002", "SCHEMA003"
    Severity Severity,
    string What, // what is happening
    string Fix, // what to do
    string? FixSql, // ready-to-run SQL,
    string Cost,  // what the fix costs
    string SkipWhen, // when to leave it alone
    int? NodeId, //  plan findings:  which operator to highlight
    string? ObjectName = null // schema findings: which table or column
);
