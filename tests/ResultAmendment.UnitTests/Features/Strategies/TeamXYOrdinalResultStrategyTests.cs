using ResultAmendment.Features.SetResultsStrategy.Strategies;
using ResultAmendment.Models;

namespace ResultAmendment.UnitTests.Features.Strategies;

public class TeamXYOrdinalResultStrategyTests
{
    private readonly TeamXYOrdinalResultStrategy strategy;

    public TeamXYOrdinalResultStrategyTests()
    {
        strategy = new TeamXYOrdinalResultStrategy();
    }

    [Fact]
    public void Generate_ShouldReturnCorrectResults_WhenInputsAreValid()
    {
        // Arrange
        var resultChanged = new ResultChanged
        {
            ResourceId = "12340402",
            HomeValue = "A",
        };

        var resourceItem = new ResourceItem
        {
            ResourceId = "12340402",
            HomeValue = "N",
        };

        // Act
        var (wrongResult, actualResult) = strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.Equal("12340402", wrongResult.ResourceId);
        Assert.Equal("12340402", actualResult.ResourceId);
        Assert.Equal("A", wrongResult.Home);
        Assert.Equal("None", actualResult.Home);
        Assert.Empty(wrongResult.Away);
        Assert.Empty(actualResult.Away);
        Assert.Equal("1234", wrongResult.RefBetTypeId);
        Assert.Equal("1234", actualResult.RefBetTypeId);
        Assert.Equal("4", wrongResult.SetX);
        Assert.Equal("4", actualResult.SetX);
        Assert.Equal("2", wrongResult.PointY);
        Assert.Equal("2", actualResult.PointY);
        Assert.Equal("K2", wrongResult.Ordinal);
        Assert.Equal("K2", actualResult.Ordinal);
    }

    [Fact]
    public void Generate_ShouldHandleNullAwayValues()
    {
        // Arrange
        var resultChanged = new ResultChanged
        {
            ResourceId = "12340415",
            HomeValue = "",
            AwayValue = null
        };

        var resourceItem = new ResourceItem
        {
            ResourceId = "12340415",
            HomeValue = "",
            AwayValue = null
        };

        // Act
        var (wrongResult, actualResult) = strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.Empty(wrongResult.Home);
        Assert.Empty(actualResult.Home);
        Assert.Empty(wrongResult.Away);
        Assert.Empty(actualResult.Away);
        Assert.Equal("4", wrongResult.SetX);
        Assert.Equal("4", actualResult.SetX);
        Assert.Equal("15", wrongResult.PointY);
        Assert.Equal("15", actualResult.PointY);
        Assert.Equal("K4", wrongResult.Ordinal);
        Assert.Equal("K4", actualResult.Ordinal);
    }
}
