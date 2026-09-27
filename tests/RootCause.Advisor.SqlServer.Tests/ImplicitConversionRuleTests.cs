using AwesomeAssertions;


namespace RootCause.Advisor.SqlServer.Tests;

public class ImplicitConversionRuleTests
{
    private static readonly TargetContext SqlServer2025 = new(17, null, 170);
    private readonly ImplicitConversionRule _rule = new();

    [Fact]
    public void Check_NvarcharAgainstVarcharColumn_ReturnsOneFinding()
    {
        var plan = TestPlansHelper.LoadPlan("plan002.sqlplan");

        var findings = _rule.Check(plan, SqlServer2025);

        var finding = findings.Should().ContainSingle().Which;
        finding.Code.Should().Be("PLAN002");
        finding.Severity.Should().Be(Severity.Warning);
    }

    [Fact]
    public void Check_MatchingTypes_ReturnsNoFindings()
    {
        var plan = TestPlansHelper.LoadPlan("plan002-NotFired.sqlplan");

        var findings = _rule.Check(plan, SqlServer2025);

        findings.Should().BeEmpty();
    }
}
