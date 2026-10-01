using Microsoft.Extensions.DependencyInjection;

namespace RootCause.Cli;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRootCauseCli(this IServiceCollection services)
    {
        services.AddKeyedSingleton<IReportWriter, TextReportWriter>(ReportFormat.Text);
        services.AddKeyedSingleton<IReportWriter, JsonReportWriter>(ReportFormat.Json);
        return services;
    }
}
