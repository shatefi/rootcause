namespace RootCause.Cli;

/// <summary>
/// The lowest severity that makes <c>analyze</c> return <see cref="ExitCodes.Findings"/>.
/// </summary>
internal enum FailOn
{
    Warning,
    Critical
}
