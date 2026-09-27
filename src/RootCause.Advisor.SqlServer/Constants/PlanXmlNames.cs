namespace RootCause.Advisor.SqlServer;

internal static class PlanXmlNames
{

    internal static class Elements
    {
        public const string RelOp = nameof(RelOp);
        public const string RunTimeInformation = nameof(RunTimeInformation);
        public const string PlanAffectingConvert = nameof(PlanAffectingConvert);
        public const string StmtSimple = nameof(StmtSimple);
    }

    internal static class Attributes
    {
        public const string NodeId = nameof(NodeId);
        public const string ConvertIssue = nameof(ConvertIssue);

        public const string Expression = nameof(Expression);
        public const string Build = nameof(Build);
        public const string CardinalityEstimationModelVersion = nameof(CardinalityEstimationModelVersion);
    }

    internal static class Values
    {
        public const string SeekPlan = "Seek Plan";
    }


}
