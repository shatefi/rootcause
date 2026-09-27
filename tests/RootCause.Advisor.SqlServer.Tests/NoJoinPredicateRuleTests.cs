using AwesomeAssertions;


namespace RootCause.Advisor.SqlServer.Tests;

public class NoJoinPredicateRuleTests
{
    private readonly NoJoinPredicateRule _rule = new();

    [Fact]
    public void Check_NoJoinPredicateRule_ReturnsNoFindings()
    {
        var plan = TestPlansHelper.LoadPlan("plan020-NotFired.sqlplan");
        var target = plan.ReadTargetContext();

        var findings = _rule.Check(plan, target);

        findings.Should().BeEmpty();
    }
}
