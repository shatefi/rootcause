using AwesomeAssertions;


namespace RootCause.Advisor.SqlServer.Tests;

public class ScalarFunctionRuleTests
{
    private readonly ScalarFunctionRule _rule = new();

    [Fact]
    public void Check_ScalarFunctionRule_ReturnsNoFindings()
    {
        var plan = TestPlansHelper.LoadPlan("plan022-NotFired.sqlplan");
        var target = plan.ReadTargetContext();

        var findings = _rule.Check(plan, target);

        findings.Should().BeEmpty();
    }
}
