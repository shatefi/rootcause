using AwesomeAssertions;


namespace RootCause.Advisor.SqlServer.Tests;

public class ParameterSniffingRuleTests
{
    private readonly ParameterSniffingRule _rule = new();

    [Fact]
    public void Check_ParameterSniffing_ReturnsNoFindings()
    {
        var plan = TestPlansHelper.LoadPlan("plan016-NotFired.sqlplan");
        var target = plan.ReadTargetContext();

        var findings = _rule.Check(plan, target);

        findings.Should().BeEmpty();
    }
}
