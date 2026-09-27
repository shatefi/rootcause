
using System.Xml.Linq;

namespace RootCause.Advisor.SqlServer;

public sealed class MissingStatisticsRule : PlanRule
{
    public override string Code => "PLAN015";

    protected override Description Describe(XElement match, TargetContext target)
    {
        var columns = string.Join(", ", match.Elements(PlanDocument.Ns + PlanXmlNames.Elements.ColumnReference)
            .Select(column => $"{(string?)column.Attribute(PlanXmlNames.Attributes.Table)}." +
                              $"{(string?)column.Attribute(PlanXmlNames.Attributes.Column)}"));

        return new Description(
            Severity: Severity.Warning,
            What: $"SQL Server has no statistics on {columns}, so it guessed how many rows the filter keeps. " +
                  "A wrong guess leads to the wrong join type, memory grant or index.",
            Fix: "Check that AUTO_CREATE_STATISTICS is ON for the database. If it has to stay off, create " +
                 "statistics on these columns with CREATE STATISTICS.",
            FixSql: null,
            Cost: "Very small: statistics take little space, and SQL Server keeps them up to date as data changes.",
            SkipWhen: "The database is read-only, or automatic statistics are off on purpose: then create only the " +
                      "statistics your important queries need.");
    }

    protected override IEnumerable<XElement> FindMatches(PlanDocument plan)
    {
        return plan.Descendants(PlanXmlNames.Elements.ColumnsWithNoStatistics);
    }
}
