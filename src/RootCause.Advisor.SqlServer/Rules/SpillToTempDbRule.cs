
using System.Xml.Linq;

namespace RootCause.Advisor.SqlServer;

public sealed class SpillToTempDbRule : PlanRule
{
    public override string Code => "PLAN018";

    protected override Description Describe(XElement match, TargetContext target)
    {
        var operatorName = (string?)match.Ancestors(PlanDocument.Ns + PlanXmlNames.Elements.RelOp)
                                         .FirstOrDefault()?
                                         .Attribute(PlanXmlNames.Attributes.PhysicalOp) ?? "operator";
        var level = (string?)match.Attribute(PlanXmlNames.Attributes.SpillLevel);
        var levelText = level is null ? "" : $" (spill level {level})";

        return new Description(
            Severity: Severity.Warning,
            What: $"The {operatorName} did not fit in its memory grant and wrote rows to tempdb{levelText}. " +
                  "Reading and writing tempdb is far slower than working in memory.",
            Fix: "The grant is sized from the estimated row count, so fix a low estimate first: update statistics, " +
                 "and remove functions or type conversions on filtered columns. For a sort, an index that returns " +
                 "the rows already in order removes the sort altogether.",
            FixSql: null,
            Cost: "Updating statistics is cheap. An index costs storage and slows every write to the table.",
            SkipWhen: "The query runs rarely and the spill is a single level: it may cost only milliseconds.");
    }

    protected override IEnumerable<XElement> FindMatches(PlanDocument plan)
    {
        return plan.Descendants(PlanXmlNames.Elements.SpillToTempDb);
    }
}
