using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Features.SetResultsStrategy.Strategies;

namespace ResultAmendment.UnitTests.Features.Strategies;

public class MapXYesNoResultStrategyTests
{
    private readonly MapXYesNoResultStrategy _strategy;

    public MapXYesNoResultStrategyTests()
    {
        _strategy = new MapXYesNoResultStrategy();
    }

    [Fact]
    public void Generate_ShouldReturnCorrectResults_WhenInputsAreValid()
    {
        // Arrange
        var resultChanged = new ResultChanged
        {
            ResourceId = "ABC12",
            HomeValue = "Y",
            AwayValue = "N"
        };

        var resourceItem = new ResourceItem
        {
            ResourceId = "ABC12",
            HomeValue = "N",
            AwayValue = "Y"
        };

        // Act
        var (wrongResult, actualResult) = _strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.NotNull(wrongResult);
        Assert.Equal("ABC12", wrongResult.ResourceId);
        Assert.Equal("Y", wrongResult.Home);
        Assert.Equal("N", wrongResult.Away);
        Assert.Equal("ABC", wrongResult.RefBetTypeId);
        Assert.Equal("12", wrongResult.SetX);

        Assert.NotNull(actualResult);
        Assert.Equal("ABC12", actualResult.ResourceId);
        Assert.Equal("N", actualResult.Home);
        Assert.Equal("Y", actualResult.Away);
        Assert.Equal("ABC", actualResult.RefBetTypeId);
        Assert.Equal("12", actualResult.SetX);
    }

    [Fact]
    public void Generate_ShouldNormalizeHomeValueToNone_WhenInputIsNONE()
    {
        // Arrange
        var resultChanged = new ResultChanged
        {
            ResourceId = "DEF34",
            HomeValue = "NONE",
            AwayValue = "N"
        };

        var resourceItem = new ResourceItem
        {
            ResourceId = "DEF34",
            HomeValue = "NONE",
            AwayValue = "Y"
        };

        // Act
        var (wrongResult, actualResult) = _strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.Equal("NONE", wrongResult.Home);
        Assert.Equal("NONE", actualResult.Home);
    }

    [Fact]
    public void Generate_ShouldHandleNullHomeValue()
    {
        // Arrange
        var resultChanged = new ResultChanged
        {
            ResourceId = "XYZ99",
            HomeValue = null,
            AwayValue = "Y"
        };

        var resourceItem = new ResourceItem
        {
            ResourceId = "XYZ99",
            HomeValue = null,
            AwayValue = "N"
        };

        // Act
        var (wrongResult, actualResult) = _strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.Equal(string.Empty, wrongResult.Home);
        Assert.Equal(string.Empty, actualResult.Home);
    }

}
