namespace RootCause.Advisor.SqlServer;

public interface IRule
{
    public string Code { get; }
    public IEnumerable<Finding> Check(PlanDocument plan, TargetContext target);
}
