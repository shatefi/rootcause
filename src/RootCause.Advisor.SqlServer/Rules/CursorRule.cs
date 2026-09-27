
using System.Xml.Linq;

namespace RootCause.Advisor.SqlServer;

public sealed class CursorRule : PlanRule
{
    public override string Code => "PLAN023";

    protected override Description Describe(XElement match, TargetContext target)
    {
        var name = (string?)match.Element(PlanDocument.Ns + PlanXmlNames.Elements.CursorPlan)?
                                 .Attribute(PlanXmlNames.Attributes.CursorName);
        var cursor = name is null ? "The cursor" : $"The cursor {name}";

        return new Description(
            Severity: Severity.Warning,
            What: $"{cursor} processes rows one at a time, so the work grows with every row. A single set-based " +
                  "statement would handle all the rows at once.",
            Fix: "Replace the loop with one set-based statement: UPDATE … FROM, INSERT … SELECT, MERGE or a window " +
                 "function. If a cursor is really needed, declare it LOCAL FAST_FORWARD.",
            FixSql: null,
            Cost: "A rewrite takes development time and must be tested to return the same results.",
            SkipWhen: "The cursor loops over a handful of rows, or each step must run separately on purpose, for " +
                      "example one maintenance command per database.");
    }

    protected override IEnumerable<XElement> FindMatches(PlanDocument plan)
    {
        return plan.Descendants(PlanXmlNames.Elements.StmtCursor);
    }
}
