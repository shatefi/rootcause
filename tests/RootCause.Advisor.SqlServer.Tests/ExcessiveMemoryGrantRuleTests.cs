using AwesomeAssertions;


namespace RootCause.Advisor.SqlServer.Tests;

public class ExcessiveMemoryGrantRuleTests
{
    private readonly ExcessiveMemoryGrantRule _rule = new();

    [Fact]
    public void Check_ExcessiveMemoryGrantRule_ReturnsNoFindings()
    {
        var plan = TestPlansHelper.LoadPlan("plan019-NotFired.sqlplan");
        var target = plan.ReadTargetContext();

        var findings = _rule.Check(plan, target);

        findings.Should().BeEmpty();
    }

    [Fact]
    public void Check_UsedMoreThanGranted_ReturnsNoFindings()
    {
        var plan = TestPlansHelper.Plan("<StmtSimple><QueryPlan><Warnings>" +
            "<MemoryGrantWarning GrantWarningKind='Used More Than Granted' RequestedMemory='1024' GrantedMemory='1024' MaxUsedMemory='4096'/>" +
            "</Warnings></QueryPlan></StmtSimple>");
             var target = plan.ReadTargetContext();

        var findings = _rule.Check(plan, target);

        findings.Should().BeEmpty();
    }
}
