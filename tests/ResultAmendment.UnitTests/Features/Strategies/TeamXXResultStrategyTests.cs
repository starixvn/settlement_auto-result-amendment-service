using ResultAmendment.Features.SetResultsStrategy.Strategies;
using ResultAmendment.Models;

namespace ResultAmendment.UnitTests.Features.Strategies;
public class TeamXXResultStrategyTests
{
    private readonly TeamXXResultStrategy _strategy;

    public TeamXXResultStrategyTests()
    {
        _strategy = new TeamXXResultStrategy();
    }

    [Fact]
    public void Generate_ShouldReturnCorrectResults_WhenInputsAreValid()
    {
        // Arrange
        var resultChanged = new ResultChanged
        {
            ResourceId = "123402",
            HomeValue = "A",
            AwayValue = ""
        };

        var resourceItem = new ResourceItem
        {
            ResourceId = "123402",
            HomeValue = "H",
            AwayId = "1111",
            HomeId = "2222"
        };

        // Act
        var (wrongResult, actualResult) = _strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.Equal("123402", wrongResult.ResourceId);
        Assert.Equal("123402", actualResult.ResourceId);
        Assert.Equal("A", wrongResult.Home);
        Assert.Equal("H", actualResult.Home);
        Assert.Equal("", wrongResult.Away);
        Assert.Equal("", actualResult.Away);
        Assert.Equal("1234", wrongResult.RefBetTypeId);
        Assert.Equal("1234", actualResult.RefBetTypeId);
        Assert.Equal("2", wrongResult.SetX);
        Assert.Equal("2", actualResult.SetX);
        Assert.Equal("2222", wrongResult.TeamIds);
        Assert.Equal("1111", actualResult.TeamIds);
    }

    [Fact]
    public void Generate_ShouldHandleNullAwayValues()
    {
        // Arrange
        var resultChanged = new ResultChanged
        {
            ResourceId = "1234502",
            HomeValue = "A",
            AwayValue = null
        };

        var resourceItem = new ResourceItem
        {
            ResourceId = "1234502",
            HomeValue = "N",
            AwayValue = null
        };

        // Act
        var (wrongResult, actualResult) = _strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.Equal("A", wrongResult.Home);
        Assert.Equal("None", actualResult.Home);
        Assert.Equal("", wrongResult.Away);
        Assert.Equal("", actualResult.Away);
    }
}