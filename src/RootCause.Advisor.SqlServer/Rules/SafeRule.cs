
using Microsoft.Extensions.Logging;

namespace RootCause.Advisor.SqlServer;

public sealed class SafeRule(IRule rule, ILogger<SafeRule> logger) : IRule
{
    public string Code => rule.Code;

    public IEnumerable<Finding> Check(PlanDocument plan, TargetContext target)
    {
        try
        {
            var findings = rule.Check(plan, target).ToList();
            return findings;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Rule {Code} failed; the other rules still ran.", rule.Code);
            return [];
        }
    }
}
