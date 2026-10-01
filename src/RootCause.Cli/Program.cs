using Microsoft.Extensions.DependencyInjection;

using RootCause.Advisor.SqlServer;
using RootCause.Cli;

using var provider = new ServiceCollection()
    .AddLogging()
    .AddRootCauseSqlServer()
    .AddRootCauseCli()
    .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

return await RootCauseCli.RunAsync(args, provider);
