using Microsoft.Extensions.DependencyInjection;

using RootCause.Advisor.SqlServer;

Console.WriteLine("RootCause is ready ...");

using var provider = new ServiceCollection()
    .AddLogging()
    .AddRootCauseSqlServer()
    .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

