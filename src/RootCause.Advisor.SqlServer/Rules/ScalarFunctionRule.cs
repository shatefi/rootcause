
using System.Xml.Linq;

namespace RootCause.Advisor.SqlServer;

public sealed class ScalarFunctionRule : PlanRule
{
    public override string Code => "PLAN022";

    protected override Description Describe(XElement match, TargetContext target)
    {
        var inlining = target.CeModelVersion >= 150
            ? "At this compatibility level SQL Server inlines many scalar functions automatically, but not this " +
              "one: check its body against the inlining requirements, or rewrite it."
            : "From compatibility level 150, SQL Server inlines many scalar functions automatically; raising the " +
              "level may fix this without a rewrite.";

        return new Description(
            Severity: Severity.Warning,
            What: "A scalar user-defined function in this query forces the whole plan to run on one thread, and " +
                  "the function runs once for every row.",
            Fix: "Move the function's logic into the query itself, or rewrite it as an inline table-valued " +
                 $"function. {inlining}",
            FixSql: null,
            Cost: "Rewriting a function changes every query that uses it. Raising the compatibility level changes " +
                  "how every query in the database is optimized, so test it first.",
            SkipWhen: "The query reads few rows: running on several threads would not help it.");
    }

    protected override IEnumerable<XElement> FindMatches(PlanDocument plan)
    {
        return plan.Descendants(PlanXmlNames.Elements.QueryPlan).Where(match => (string?)match.Attribute(PlanXmlNames.Attributes.NonParallelPlanReason) == PlanXmlNames.Values.TSQLUserDefinedFunctionsNotParallelizable);
    }
}
