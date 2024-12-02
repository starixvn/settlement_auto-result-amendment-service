namespace ResultAmendment.UnitTests.Features.Strategies;

using ResultAmendment.Features.SetResultsStrategy.Strategies;
using ResultAmendment.Models;
using Xunit;

public class TimeResultStrategyTests
{
    private readonly TimeResultStrategy strategy;

    public TimeResultStrategyTests()
    {
        strategy = new TimeResultStrategy();
    }

    [Fact]
    public void Generate_ShouldReturnCorrectResults_WhenInputsAreValid()
    {
        // Arrange
        var resultChanged = new ResultChanged
        {
            ResourceId = "906801",
            HomeValue = "12",
            AwayValue = "12"
        };

        var resourceItem = new ResourceItem
        {
            ResourceId = "906801",
            HomeValue = "1",
            AwayValue = "12",
            GroupBetType = 5
        };

        // Act
        var (wrongResult, actualResult) = strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.Equal("906801", wrongResult.ResourceId);
        Assert.Equal("906801", actualResult.ResourceId);

        Assert.Equal("12", wrongResult.Home);
        Assert.Equal("01", actualResult.Home);

        Assert.Equal("12", wrongResult.Away);
        Assert.Equal("12", actualResult.Away);

        Assert.Equal("9068", wrongResult.RefBetTypeId);
        Assert.Equal("9068", actualResult.RefBetTypeId);

        Assert.Equal("1", wrongResult.SetX);
        Assert.Equal("1", actualResult.SetX);

        Assert.Equal("5", wrongResult.GroupBetType);
        Assert.Equal("5", actualResult.GroupBetType);
    }

}