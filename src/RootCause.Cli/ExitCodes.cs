namespace RootCause.Cli;

internal static class ExitCodes
{
    /// <summary>The analysis ran and found nothing at the <c>--fail-on</c> level.</summary>
    public const int Success = 0;

    /// <summary>The analysis ran and found problems at or above the <c>--fail-on</c> level.</summary>
    public const int Findings = 1;

    /// <summary>The tool could not do its job: bad input, unreadable plan, or an unexpected failure.</summary>
    public const int Error = 2;
}
