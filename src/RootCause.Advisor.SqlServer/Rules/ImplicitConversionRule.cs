
using System.Xml.Linq;

namespace RootCause.Advisor.SqlServer;

public sealed class ImplicitConversionRule : PlanRule
{
    public override string Code => "PLAN002";

    protected override Description Describe(XElement match, TargetContext target)
    {
        var expression = (string?)match.Attribute(PlanXmlNames.Attributes.Expression);
        return new Description(
            Severity: Severity.Warning,
        What: $"SQL Server converts the column to compare it with a value of another type ({expression}), " +
              "so it cannot seek the index on that column and reads every row instead.",
        Fix: "Declare the parameter or variable with the column's own type, for example varchar instead of " +
             "nvarchar. In .NET, set the parameter's SqlDbType instead of using AddWithValue.",
        FixSql: null,
        Cost: "Usually none: changing a parameter's type is a one-line change in the calling code. " +
              "Changing the column's type instead rewrites the whole table.",
        SkipWhen: "The table is small or the query runs rarely: the extra scan then costs very little.");
    }

    protected override IEnumerable<XElement> FindMatches(PlanDocument plan)
    {
        return plan.Descendants(PlanXmlNames.Elements.PlanAffectingConvert).Where(match => (string?)match.Attribute(PlanXmlNames.Attributes.ConvertIssue) == PlanXmlNames.Values.SeekPlan);
    }
}
