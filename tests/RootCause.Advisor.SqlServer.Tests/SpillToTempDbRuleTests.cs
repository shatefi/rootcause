using AwesomeAssertions;


namespace RootCause.Advisor.SqlServer.Tests;

public class SpillToTempDbRuleTests
{
    private readonly SpillToTempDbRule _rule = new();

    [Fact]
    public void Check_SpillToTempDbRule_ReturnsNoFindings()
    {
        var plan = TestPlansHelper.LoadPlan("plan018-NotFired.sqlplan");
        var target = plan.ReadTargetContext();

        var findings = _rule.Check(plan, target);

        findings.Should().BeEmpty();
    }
}
