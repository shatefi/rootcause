
using System.Xml.Linq;

namespace RootCause.Advisor.SqlServer;

public sealed class MissingIndexRule : PlanRule
{
    public override string Code => "PLAN007";

    private const double WarningImpact = 50;

    protected override Description Describe(XElement match, TargetContext target)
    {
        var impact = (double?)match.Attribute(PlanXmlNames.Attributes.Impact) ?? 0;
        var index = match.Element(PlanDocument.Ns + PlanXmlNames.Elements.MissingIndex);
        var table = $"{(string?)index?.Attribute(PlanXmlNames.Attributes.Schema)}." +
                    $"{(string?)index?.Attribute(PlanXmlNames.Attributes.Table)}";

        return new Description(
            Severity: impact >= WarningImpact ? Severity.Warning : Severity.Suggestion,
            What: $"SQL Server estimates that an index on {table} would cut this query's cost by about {impact:0}%.",
            Fix: "Compare the suggestion with the indexes the table already has before creating it. If one already " +
                 "starts with the same columns, widen that index instead of adding a near-duplicate.",
            FixSql: null,
            Cost: "Every index takes storage and slows every INSERT, UPDATE and DELETE on the table.",
            SkipWhen: "The table is written far more often than it is read, the query runs rarely, or an existing " +
                      "index already covers most of it.");
    }

    protected override IEnumerable<XElement> FindMatches(PlanDocument plan)
    {
        return plan.Descendants(PlanXmlNames.Elements.MissingIndexGroup)
               .GroupBy(group => group.Element(PlanDocument.Ns + PlanXmlNames.Elements.MissingIndex)?
                                      .ToString(SaveOptions.DisableFormatting))
               .Select(sameIndex => sameIndex.MaxBy(group => (double?)group.Attribute(PlanXmlNames.Attributes.Impact) ?? 0)!);
    }
}
