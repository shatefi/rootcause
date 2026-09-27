namespace RootCause.Advisor.SqlServer;

public sealed record TargetContext(int ProductMajorVersion, int? CompatibilityLevel, int? CeModelVersion);
