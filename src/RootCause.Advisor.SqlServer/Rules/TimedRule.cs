
using System.Diagnostics;

using Microsoft.Extensions.Logging;

namespace RootCause.Advisor.SqlServer;

public sealed class TimedRule(IRule rule, ILogger<TimedRule> logger) : IRule
{
    public string Code => rule.Code;

    public IEnumerable<Finding> Check(PlanDocument plan, TargetContext target)
    {
        var time = Stopwatch.GetTimestamp();
        var findings = rule.Check(plan, target).ToList();
        logger.LogDebug("Rule {Code} took {Ms:F1} ms", rule.Code, Stopwatch.GetElapsedTime(time).TotalMilliseconds);
        return findings;

    }
}
