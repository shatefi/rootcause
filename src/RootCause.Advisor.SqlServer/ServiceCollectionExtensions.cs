
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace RootCause.Advisor.SqlServer;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddRootCauseSqlServer(this IServiceCollection services)
    {
        var allRules = typeof(IRule).Assembly.GetTypes()
        .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(PlanRule).IsAssignableFrom(t));

        foreach (var rule in allRules)
        {
            services.AddSingleton<IRule>(sp => new SafeRule(new TimedRule(
                (IRule)ActivatorUtilities.CreateInstance(sp, rule),
                 sp.GetRequiredService<ILogger<TimedRule>>()),
                 sp.GetRequiredService<ILogger<SafeRule>>()));
        }

        services.AddSingleton<Analyzer>();
        return services;
    }
}
