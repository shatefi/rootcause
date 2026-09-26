using System.Xml.Linq;

using AwesomeAssertions;


namespace RootCause.Advisor.SqlServer.Tests;


public class PlanDocumentTests
{

    [Fact]
    public void HasRuntimeStats_ActualPlan_ReturnsTrue()
    {
        //Arrange
        var plan = TestPlansHelper.LoadPlan();

        //Act
        bool actual = plan.HasRuntimeStats;

        //Assert
        actual.Should().BeTrue();
    }
    [Fact]
    public void Descendants_RelOp_ReturnsEveryOperator()
    {
        //Arrange
        var plan = TestPlansHelper.LoadPlan();

        //Act
        var actual = plan.Descendants("RelOp");

        //Assert
        actual.Should().HaveCount(1);
    }

    [Fact]
    public void ReadTargetContext_Compat170Plan_ReadsVersionAndCeModel()
    {
        var plan = TestPlansHelper.LoadPlan();

        var actualTarget = plan.ReadTargetContext();

        actualTarget.Should().Be(new TargetContext(17, null, 170));
    }
}
