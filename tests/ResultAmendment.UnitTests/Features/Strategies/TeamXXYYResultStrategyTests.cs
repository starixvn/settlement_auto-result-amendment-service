using ResultAmendment.Features.SetResultsStrategy.Strategies;
using ResultAmendment.Models;

namespace ResultAmendment.UnitTests.Features.Strategies;

public class TeamXXYYResultStrategyTests
{
    private readonly TeamXXYYResultStrategy strategy = new();

    [Fact]
    public void Generate_ShouldReturnCorrectResults_WhenInputsAreValid()
    {
        // Arrange
        var resultChanged = new ResultChanged
        {
            ResourceId = "12340115",
            HomeValue = "A",
            AwayValue = ""
        };

        var resourceItem = new ResourceItem
        {
            ResourceId = "12340115",
            HomeValue = "N",
            AwayValue = "",
            HomeId = "2222",
            AwayId = "1111"
        };

        // Act
        var (wrongResult, actualResult) = strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.Equal("12340115", wrongResult.ResourceId);
        Assert.Equal("12340115", actualResult.ResourceId);
        Assert.Equal("A", wrongResult.Home);
        Assert.Equal("None", actualResult.Home);
        Assert.Equal("", wrongResult.Away);
        Assert.Equal("", actualResult.Away);
        Assert.Equal("1234", wrongResult.RefBetTypeId);
        Assert.Equal("1234", actualResult.RefBetTypeId);
        Assert.Equal("1", wrongResult.SetX);
        Assert.Equal("1", actualResult.SetX);
        Assert.Equal("15", wrongResult.PointY);
        Assert.Equal("15", actualResult.PointY);
    }

    [Fact]
    public void Generate_ShouldHandleNullAwayValues()
    {
        // Arrange
        var resultChanged = new ResultChanged
        {
            ResourceId = "12340115",
            HomeValue = "TeamA",
            AwayValue = null
        };

        var resourceItem = new ResourceItem
        {
            ResourceId = "12340115",
            HomeValue = "TeamC",
            AwayValue = null
        };

        // Act
        var (wrongResult, actualResult) = strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.Equal("TeamA", wrongResult.Home);
        Assert.Equal("TeamC", actualResult.Home);
        Assert.Equal("", wrongResult.Away);
        Assert.Equal("", actualResult.Away);
    }
}