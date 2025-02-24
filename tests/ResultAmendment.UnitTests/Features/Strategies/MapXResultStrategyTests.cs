using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Features.SetResultsStrategy.Strategies;

namespace ResultAmendment.UnitTests.Features.Strategies;

public class MapXResultStrategyTests
{
    private readonly MapXResultStrategy strategy;

    public MapXResultStrategyTests()
    {
        strategy = new MapXResultStrategy();
    }

    [Fact]
    public void Generate_ShouldReturnCorrectResults_WhenInputsAreValid()
    {
        // Arrange
        var resultChanged = new ResultChanged
        {
            ResourceId = "ABC12",
            HomeValue = "2.5",
            AwayValue = "1.5"
        };

        var resourceItem = new ResourceItem
        {
            ResourceId = "ABC12",
            HomeValue = "2.8",
            AwayValue = "1.7"
        };

        // Act
        var (wrongResult, actualResult) = strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.NotNull(wrongResult);
        Assert.Equal("ABC12", wrongResult.ResourceId);
        Assert.Equal("2.5", wrongResult.Home);
        Assert.Equal("1.5", wrongResult.Away);
        Assert.Equal("ABC", wrongResult.RefBetTypeId);
        Assert.Equal("12", wrongResult.SetX);

        Assert.NotNull(actualResult);
        Assert.Equal("ABC12", actualResult.ResourceId);
        Assert.Equal("2.8", actualResult.Home);
        Assert.Equal("1.7", actualResult.Away);
        Assert.Equal("ABC", actualResult.RefBetTypeId);
        Assert.Equal("12", actualResult.SetX);
    }

    [Fact]
    public void Generate_ShouldHandleMissingAwayValue()
    {
        // Arrange
        var resultChanged = new ResultChanged
        {
            ResourceId = "XYZ99",
            HomeValue = "3.0",
            AwayValue = string.Empty
        };

        var resourceItem = new ResourceItem
        {
            ResourceId = "XYZ99",
            HomeValue = "3.1",
        };

        // Act
        var (wrongResult, actualResult) = strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.NotNull(wrongResult);
        Assert.Equal("XYZ99", wrongResult.ResourceId);
        Assert.Equal("3.0", wrongResult.Home);
        Assert.Equal("", wrongResult.Away);
        Assert.Equal("XYZ", wrongResult.RefBetTypeId);
        Assert.Equal("99", wrongResult.SetX);

        Assert.NotNull(actualResult);
        Assert.Equal("XYZ99", actualResult.ResourceId);
        Assert.Equal("3.1", actualResult.Home);
        Assert.Equal("", actualResult.Away);
        Assert.Equal("XYZ", actualResult.RefBetTypeId);
        Assert.Equal("99", actualResult.SetX);
    }
}