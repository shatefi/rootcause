using AwesomeAssertions;


namespace RootCause.Advisor.SqlServer.Tests;

public class MissingIndexRuleTests
{
    private readonly MissingIndexRule _rule = new();

    [Fact]
    public void Check_MissingIndex_ReturnsNoFindings()
    {
        var plan = TestPlansHelper.LoadPlan("plan007-NotFired.sqlplan");
        var target = plan.ReadTargetContext();

        var findings = _rule.Check(plan, target);

        findings.Should().BeEmpty();
    }

    [Fact]
    public void Check_SameIndexSuggestedTwice_ReturnsOneFinding()
    {
        var plan = TestPlansHelper.LoadPlan("plan007.sqlplan");
        var target = plan.ReadTargetContext();

        var findings = _rule.Check(plan, target);

        findings.Count().Should().Be(1);
    }
}
