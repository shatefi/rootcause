
using System.Xml.Linq;

namespace RootCause.Advisor.SqlServer;

public sealed class NoJoinPredicateRule : PlanRule
{
    public override string Code => "PLAN020";

    protected override Description Describe(XElement match, TargetContext target)
    {
        return new Description(
            Severity: Severity.Critical,
            What: "A join has no join condition, so SQL Server matches every row of one input with every row " +
                  "of the other. The result is almost certainly wrong, and its size is rows × rows.",
            Fix: "Add the missing ON condition, or, in an old-style comma join, the missing condition in the " +
                 "WHERE clause.",
            FixSql: null,
            Cost: "None: one line of SQL.",
            SkipWhen: "The cross join is deliberate, for example building a calendar or numbers table from two " +
                      "small inputs.");
    }

    protected override IEnumerable<XElement> FindMatches(PlanDocument plan)
    {
        return plan.Descendants(PlanXmlNames.Elements.Warnings).Where(match=> (bool?)match.Attribute(PlanXmlNames.Attributes.NoJoinPredicate) ==true);
    }
}
