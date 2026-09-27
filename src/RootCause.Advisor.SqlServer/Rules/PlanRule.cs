
using System.Xml.Linq;

namespace RootCause.Advisor.SqlServer;

public abstract class PlanRule : IRule
{
    public abstract string Code { get; }
    private static int? NodeIdOf(XElement match) => (int?)match.AncestorsAndSelf(PlanDocument.Ns + PlanXmlNames.Elements.RelOp)
    .FirstOrDefault()?.Attribute(PlanXmlNames.Attributes.NodeId);

    public IEnumerable<Finding> Check(PlanDocument plan, TargetContext target)
    {
        if (!AppliesTo(plan, target))
        {
            return [];
        }

        return FindMatches(plan).Select(match =>
        {
            var description = Describe(match, target);
            return new Finding(Code,
             description.Severity,
             description.What,
             description.Fix,
             description.FixSql,
             description.Cost,
             description.SkipWhen,
            NodeId: NodeIdOf(match));
        });
    }

    protected virtual bool AppliesTo(PlanDocument plan, TargetContext target) => true;
    protected abstract IEnumerable<XElement> FindMatches(PlanDocument plan);
    protected sealed record Description(Severity Severity, string What, string Fix, string? FixSql, string Cost, string SkipWhen);
    protected abstract Description Describe(XElement match, TargetContext target);
}
