
using System.Xml.Linq;

namespace RootCause.Advisor.SqlServer;

public sealed class ExcessiveMemoryGrantRule : PlanRule
{
    public override string Code => "PLAN019";

    protected override Description Describe(XElement match, TargetContext target)
    {
        var granted = (long?)match.Attribute(PlanXmlNames.Attributes.GrantedMemory) ?? 0;
        var used = (long?)match.Attribute(PlanXmlNames.Attributes.MaxUsedMemory) ?? 0;

        var feedback = target.CeModelVersion >= 150
            ? "At this compatibility level, SQL Server lowers the grant by itself when the same query runs again " +
              "(memory grant feedback)."
            : "From compatibility level 150, SQL Server lowers the grant by itself when the same query runs again " +
              "(memory grant feedback).";

        return new Description(
            Severity: Severity.Warning,
            What: $"The query was granted {granted:N0} KB of memory but used at most {used:N0} KB. Memory held and " +
                  "not used is unavailable to other queries, which may have to wait for theirs.",
            Fix: "The grant is sized from the estimated rows × their estimated width. Wide declared columns are " +
                 "assumed half full, so leave wide columns out of sorts (no SELECT *), sort narrow keys first and " +
                 $"fetch wide columns after, or use an index in the needed order. {feedback}",
            FixSql: null,
            Cost: "Fixing the estimate is usually cheap. A MAX_GRANT_PERCENT query hint caps the grant, but must " +
                  "then be maintained by hand.",
            SkipWhen: "The server has memory to spare and no queries wait for a grant (no RESOURCE_SEMAPHORE waits).");
    }

    protected override IEnumerable<XElement> FindMatches(PlanDocument plan)
    {
        return plan.Descendants(PlanXmlNames.Elements.MemoryGrantWarning).Where(match => (string?)match.Attribute(PlanXmlNames.Attributes.GrantWarningKind) == PlanXmlNames.Values.ExcessiveGrant);
    }
}
