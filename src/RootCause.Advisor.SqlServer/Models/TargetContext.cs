namespace RootCause.Advisor.SqlServer;

public sealed record class TargetContext(int ProductMajorVersion, int? CompatibilityLevel, int CeModelVersion)
{

}
