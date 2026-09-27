
using System.Xml.Linq;

namespace RootCause.Advisor.SqlServer;

public sealed class ParameterSniffingRule : PlanRule
{
    public override string Code => "PLAN016";

    protected override Description Describe(XElement match, TargetContext target)
    {
        var parameter = (string?)match.Attribute(PlanXmlNames.Attributes.Column);
        var compiled = (string?)match.Attribute(PlanXmlNames.Attributes.ParameterCompiledValue);
        var runtime = (string?)match.Attribute(PlanXmlNames.Attributes.ParameterRuntimeValue);

        var separatePlans = target.CeModelVersion >= 160
            ? "At this compatibility level, parameter sensitive plan optimization can keep separate plans for " +
              "very different values."
            : "From compatibility level 160 (SQL Server 2022), parameter sensitive plan optimization can keep " +
              "separate plans for very different values.";

        return new Description(
            Severity: Severity.Suggestion,
            What: $"The plan was compiled for {parameter} = {compiled}, but this run used {runtime}. If the two " +
                  "values match very different numbers of rows, a plan that suits one can be slow for the other.",
            Fix: "Compare estimated and actual rows. If they differ widely, use OPTION (RECOMPILE) for a query " +
                 $"that runs rarely, or OPTIMIZE FOR a typical value. {separatePlans}",
            FixSql: null,
            Cost: "RECOMPILE spends compile time on every run; OPTIMIZE FOR must be revisited as the data changes.",
            SkipWhen: "Both values match a similar number of rows: different values alone are normal and harmless.");
    }

    protected override IEnumerable<XElement> FindMatches(PlanDocument plan)
    {
        return plan.Descendants(PlanXmlNames.Elements.ParameterList)
        .Elements(PlanDocument.Ns + PlanXmlNames.Elements.ColumnReference)
        .Where(match => match.Attribute(PlanXmlNames.Attributes.ParameterCompiledValue) is { } compiled
        && match.Attribute(PlanXmlNames.Attributes.ParameterRuntimeValue) is { } runtime
        && compiled.Value != runtime.Value);
    }
}
