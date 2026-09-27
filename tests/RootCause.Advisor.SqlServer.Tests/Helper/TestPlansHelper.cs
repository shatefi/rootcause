using System.Xml.Linq;


namespace RootCause.Advisor.SqlServer.Tests;

internal static class TestPlansHelper
{
    public static PlanDocument LoadPlan(string planName = "plan002.sqlplan")
    {
        var planPath = Path.Combine(AppContext.BaseDirectory, "Fixtures", planName);
        var xml = XDocument.Load(planPath);
        var plan = new PlanDocument(xml);
        return plan;
    }

    public static PlanDocument Plan(string statements) => new(XDocument.Parse(
    "<ShowPlanXML xmlns='http://schemas.microsoft.com/sqlserver/2004/07/showplan' Build='17.0.1.1'>" +
    $"<BatchSequence><Batch><Statements>{statements}</Statements></Batch></BatchSequence></ShowPlanXML>"));
}
