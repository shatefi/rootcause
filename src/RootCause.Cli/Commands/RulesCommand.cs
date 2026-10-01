using System.CommandLine;

namespace RootCause.Cli;

public sealed class RulesCommand : Command
{
    public RulesCommand(IServiceProvider services) : base("rules", "Work with the analysis rules.")
    {
        Subcommands.Add(new ListRulesCommand(services));
    }
}
