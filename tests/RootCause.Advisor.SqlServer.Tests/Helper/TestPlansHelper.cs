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
}
