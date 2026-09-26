namespace RootCause.Advisor.SqlServer;

internal static class PlanXmlNames
{

    internal static class Elements
    {
        public const string RelOp = nameof(RelOp);
        public const string RunTimeInformation = nameof(RunTimeInformation);
    }

    internal static class Attributes
    {
        public const string NodeId = nameof(NodeId);
        public const string ConvertIssue = nameof(ConvertIssue);
    }

    internal static class Values
    {
        public const string SeekPlan = "Seek Plan";
    }


}
