namespace RootCause.Advisor.SqlServer;

public sealed class Analyzer(IEnumerable<IRule> rules)
{
    public IReadOnlyList<Finding> Analyze(PlanDocument plan)
    {
        var target = plan.ReadTargetContext();
        return [.. rules.SelectMany(rule => rule.Check(plan, target))];
    }

}
