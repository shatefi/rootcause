using System.Xml.Linq;

using AwesomeAssertions;

using RootCause.Advisor.SqlServer;


namespace RootCause.Advisor.SqlServer.Tests;


public class PlanDocumentTests
{
    private  readonly string _plan002Path= "./Fixtures/plan002.sqlplan";

    [Fact]
    public void GivenHasRuntimeStats_WhenActualPlan_ThenReturnsTrue()
    {
        //Arrange
        var xml= XDocument.Load(_plan002Path);
        var plan= new PlanDocument(xml);

        //Act
        bool actual= plan.HasRuntimeStats;

        //Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void GivenDescendants_WhenRelOp_ThenReturnsEveryOperator()
    {
        //Arrange
        var xml= XDocument.Load(_plan002Path);
        var plan= new PlanDocument(xml);

        //Act
        var actual= plan.Descendants("RelOp");

        //Assert
        actual.Count().Should().Be(2);
    }




}
