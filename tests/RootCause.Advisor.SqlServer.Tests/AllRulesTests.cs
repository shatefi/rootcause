using AwesomeAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace RootCause.Advisor.SqlServer.Tests;

public class AllRulesTests
{
    private static readonly IRule[] AllRules =
        [.. typeof(IRule).Assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } && typeof(PlanRule).IsAssignableFrom(type))
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

public class AnalyzerTests
{
    private static ServiceProvider BuildProvider() =>
    new ServiceCollection()
    .AddLogging()
    .AddRootCauseSqlServer()
    .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

    [Fact]
    public void AddRootCauseSqlServer_EveryRuleClass_IsRegisteredOnce()
    {
        using var provider = BuildProvider();

        var registeredCodes = provider.GetServices<IRule>().Select(rule => rule.Code).ToList();
        var ruleCount = typeof(PlanRule).Assembly.GetTypes()
            .Count(type => type is { IsClass: true, IsAbstract: false } && typeof(PlanRule).IsAssignableFrom(type));

        registeredCodes.Should().HaveCount(ruleCount);
        registeredCodes.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void Analyze_ImplicitConversionPlan_ReturnsItsFinding()
    {
        using var provider = BuildProvider();
        var analyzer = provider.GetRequiredService<Analyzer>();

        var findings = analyzer.Analyze(TestPlansHelper.LoadPlan("plan002.sqlplan"));

        findings.Select(finding => finding.Code).Should().Contain("PLAN002");
    }

    [Fact]
    public void Analyze_OneRuleThrows_OtherRulesStillReturnFindings()
    {
        using var provider = new ServiceCollection()
            .AddLogging()
            .AddRootCauseSqlServer()
            .AddSingleton<IRule>(sp => new SafeRule(new ThrowingRule(), sp.GetRequiredService<ILogger<SafeRule>>()))
            .BuildServiceProvider();
        var analyzer = provider.GetRequiredService<Analyzer>();

        var findings = analyzer.Analyze(TestPlansHelper.LoadPlan("plan002.sqlplan"));

        findings.Select(finding => finding.Code).Should().Contain("PLAN002");
    }

    private sealed class ThrowingRule : IRule
    {
        public string Code => "TEST001";
        public IEnumerable<Finding> Check(PlanDocument plan, TargetContext target) =>
            throw new InvalidOperationException("This rule always fails.");
    }
}
