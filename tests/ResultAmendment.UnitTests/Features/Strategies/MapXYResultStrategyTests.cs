using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Features.SetResultsStrategy.Strategies;
namespace ResultAmendment.UnitTests.Features.Strategies;


public class MapXYResultStrategyTests
{
    private readonly MapXYResultStrategy _strategy;

    public MapXYResultStrategyTests()
    {
        _strategy = new MapXYResultStrategy();
    }

    [Fact]
    public void Generate_ShouldReturnCorrectResults_WhenInputsAreValid()
    {
        // Arrange
        var resultChanged = new ResultChanged
        {
            ResourceId = "90681234",
            HomeValue = "12",
            AwayValue = "11"
        };

        var resourceItem = new ResourceItem
        {
            ResourceId = "90681234",
            HomeValue = "2",
            AwayValue = "1"
        };

        // Act
        var (wrongResult, actualResult) = _strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.NotNull(wrongResult);
        Assert.Equal("90681234", wrongResult.ResourceId);
        Assert.Equal("12", wrongResult.Home);
        Assert.Equal("11", wrongResult.Away);
        Assert.Equal("9068", wrongResult.RefBetTypeId);
        Assert.Equal("12", wrongResult.SetX);
        Assert.Equal("34", wrongResult.PointY);

        Assert.NotNull(actualResult);
        Assert.Equal("90681234", actualResult.ResourceId);
        Assert.Equal("2", actualResult.Home);
        Assert.Equal("1", actualResult.Away);
        Assert.Equal("9068", actualResult.RefBetTypeId);
        Assert.Equal("12", actualResult.SetX);
        Assert.Equal("34", actualResult.PointY);
    }
}
