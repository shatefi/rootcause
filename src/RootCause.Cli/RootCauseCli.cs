using System.CommandLine;
using System.CommandLine.Invocation;

namespace RootCause.Cli;

public static class RootCauseCli
{
    public static RootCommand Build(IServiceProvider services) =>
        new("Finds out why a SQL Server query is slow — and says what the fix costs, and when to leave it alone.")
        {
            
            new AnalyzeCommand(services),
            new RulesCommand(services)
        };

    public static async Task<int> RunAsync(
        string[] args,
        IServiceProvider services,
        InvocationConfiguration? configuration = null)
    {
        configuration ??= new InvocationConfiguration();
        configuration.EnableDefaultExceptionHandler = false;

        var parseResult = Build(services).Parse(args);

        if (parseResult.Action is ParseErrorAction)
        {
            await parseResult.InvokeAsync(configuration);
            return ExitCodes.Error;
        }

        try
        {
            return await parseResult.InvokeAsync(configuration);
        }
        catch (Exception ex)
        {
            await configuration.Error.WriteLineAsync($"RootCause: unexpected error - {ex.Message}");
            return ExitCodes.Error;
        }
    }
}
