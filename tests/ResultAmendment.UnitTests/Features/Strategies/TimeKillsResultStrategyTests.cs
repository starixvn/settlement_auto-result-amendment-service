using ResultAmendment.Features.SetResultsStrategy.Strategies;
using ResultAmendment.Models;

namespace ResultAmendment.UnitTests.Features.Strategies;


public class TimeKillsResultStrategyTests
{
    private readonly TimeKillsResultStrategy strategy;

    public TimeKillsResultStrategyTests()
    {
        strategy = new TimeKillsResultStrategy();
    }

    [Fact]
    public void Generate_ShouldReturnCorrectResults_WhenInputsAreValid()
    {
        // Arrange
        var resultChanged = new ResultChanged
        {
            ResourceId = "12340304",
            HomeValue = "1",
            AwayValue = "22"
        };

        var resourceItem = new ResourceItem
        {
            ResourceId = "12340304",
            HomeValue = "15",
            AwayValue = "22",
            GroupBetType = 5
        };

        // Act
        var (wrongResult, actualResult) = strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.Equal("12340304", wrongResult.ResourceId);
        Assert.Equal("12340304", actualResult.ResourceId);

        Assert.Equal("01", wrongResult.Home);
        Assert.Equal("15", actualResult.Home);

        Assert.Equal("22", wrongResult.Away);
        Assert.Equal("22", actualResult.Away);

        Assert.Equal("1234", wrongResult.RefBetTypeId);
        Assert.Equal("1234", actualResult.RefBetTypeId);

        Assert.Equal("3", wrongResult.SetX);
        Assert.Equal("3", actualResult.SetX);

        Assert.Equal("4", wrongResult.PointY);
        Assert.Equal("4", actualResult.PointY);

        Assert.Equal("5", wrongResult.GroupBetType);
        Assert.Equal("5", actualResult.GroupBetType);
    }
}
