
using System.Xml.Linq;

namespace RootCause.Advisor.SqlServer;

public sealed class OptimizerStoppedRule : PlanRule
{
    public override string Code => "PLAN025";

    protected override Description Describe(XElement match, TargetContext target)
    {
        var reason = (string?)match.Attribute(PlanXmlNames.Attributes.StatementOptmEarlyAbortReason);
        var why = reason == PlanXmlNames.Values.MemoryLimitExceeded
            ? "ran out of memory for compiling"
            : "ran out of time";

        return new Description(
            Severity: Severity.Warning,
            What: $"The optimizer stopped searching for a better plan because it {why}. The plan it used is the " +
                  "best one found so far, which may not be a good one.",
            Fix: "Give the optimizer less to consider: break a very large query into steps with temporary tables, " +
                 "and remove joins to tables or views the result does not need.",
            FixSql: null,
            Cost: "A rewrite takes development time and must be tested to return the same results.",
            SkipWhen: "The query is already fast enough: a search that stopped early can still have found a good plan.");
    }

    protected override IEnumerable<XElement> FindMatches(PlanDocument plan)
    {
        return plan.Xml.Descendants().Where(match => (string?)match.Attribute(PlanXmlNames.Attributes.StatementOptmEarlyAbortReason) is PlanXmlNames.Values.TimeOut or PlanXmlNames.Values.MemoryLimitExceeded);
    }
}
