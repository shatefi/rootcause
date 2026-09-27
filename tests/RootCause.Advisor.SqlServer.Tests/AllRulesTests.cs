using AwesomeAssertions;


namespace RootCause.Advisor.SqlServer.Tests;

public class AllRulesTests
{
    private static readonly IRule[] AllRules =
        [.. typeof(IRule).Assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } && typeof(IRule).IsAssignableFrom(type))
            .Select(type => (IRule)Activator.CreateInstance(type)!)];

    public static TheoryData<string[], string> PositivePlans => new()
    {
        { ["PLAN002"], "plan002.sqlplan" },
        { ["PLAN007"], "plan007.sqlplan" },
        { ["PLAN015", "PLAN019"], "plan015.sqlplan" },
        { ["PLAN016"], "plan016.sqlplan" },
        { ["PLAN018"], "plan018.sqlplan" },
        { ["PLAN019"], "plan019.sqlplan" },
        { ["PLAN020"], "plan020.sqlplan" },
        { ["PLAN022"], "plan022.sqlplan" },
        { ["PLAN023"], "plan023.sqlplan" },
        { ["PLAN025"], "plan025.sqlplan" },
    };

    [Theory, MemberData(nameof(PositivePlans))]
    public void Check_PositivePlan_ExactlyTheExpectedRulesFire(string[] expectedCodes, string fileName)
    {
        var plan = TestPlansHelper.LoadPlan(fileName);
        var target = plan.ReadTargetContext();

        var firing = AllRules.Where(rule => rule.Check(plan, target).Any())
                             .Select(rule => rule.Code);

        firing.Should().BeEquivalentTo(expectedCodes);
    }

    
}
