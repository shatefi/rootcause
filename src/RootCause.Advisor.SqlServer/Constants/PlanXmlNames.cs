namespace RootCause.Advisor.SqlServer;

internal static class PlanXmlNames
{

    internal static class Elements
    {
        public const string RelOp = nameof(RelOp);
        public const string RunTimeInformation = nameof(RunTimeInformation);
        public const string PlanAffectingConvert = nameof(PlanAffectingConvert);
        public const string StmtSimple = nameof(StmtSimple);

        public const string MissingIndexes = nameof(MissingIndexes);
        public const string MissingIndexGroup = nameof(MissingIndexGroup);
        public const string ColumnsWithNoStatistics = nameof(ColumnsWithNoStatistics);

        public const string ParameterList = nameof(ParameterList);
        public const string ColumnReference = nameof(ColumnReference);
        public const string SpillToTempDb = nameof(SpillToTempDb);
        public const string MemoryGrantWarning = nameof(MemoryGrantWarning);
        public const string NoJoinPredicate = nameof(NoJoinPredicate);
        
        public const string StmtCursor = nameof(StmtCursor);
        public const string CursorPlan = nameof(CursorPlan);
        public const string Warnings = nameof(Warnings);
        public const string QueryPlan = nameof(QueryPlan);
        public const string MissingIndex = nameof(MissingIndex);
    }

    internal static class Attributes
    {
        public const string NodeId = nameof(NodeId);
        public const string ConvertIssue = nameof(ConvertIssue);

        public const string Expression = nameof(Expression);
        public const string Build = nameof(Build);
        public const string CardinalityEstimationModelVersion = nameof(CardinalityEstimationModelVersion);
        public const string ParameterCompiledValue = nameof(ParameterCompiledValue);
        public const string ParameterRuntimeValue = nameof(ParameterRuntimeValue);
        public const string GrantWarningKind = nameof(GrantWarningKind);
        public const string SpillLevel = nameof(SpillLevel);
        public const string StatementOptmEarlyAbortReason = nameof(StatementOptmEarlyAbortReason);
        public const string Impact = nameof(Impact);
        public const string NonParallelPlanReason = nameof(NonParallelPlanReason);
        public const string NoJoinPredicate = nameof(NoJoinPredicate);
        public const string PhysicalOp = nameof(PhysicalOp);
        public const string Schema = nameof(Schema);
        public const string Table = nameof(Table);
        public const string Column = nameof(Column);
        public const string GrantedMemory = nameof(GrantedMemory);
        public const string MaxUsedMemory = nameof(MaxUsedMemory);
        public const string CursorName = nameof(CursorName);
    }

    internal static class Values
    {
        public const string SeekPlan = "Seek Plan";
        public const string ExcessiveGrant = "Excessive Grant";
        public const string TimeOut = nameof(TimeOut);
        public const string MemoryLimitExceeded = nameof(MemoryLimitExceeded);
        public const string TSQLUserDefinedFunctionsNotParallelizable = nameof(TSQLUserDefinedFunctionsNotParallelizable);
    }


}
