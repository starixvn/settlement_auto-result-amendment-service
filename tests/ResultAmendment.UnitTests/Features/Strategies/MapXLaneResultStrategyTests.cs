using ResultAmendment.Features.SetResultsStrategy.Strategies;
using ResultAmendment.Models;

namespace ResultAmendment.UnitTests.Features.Strategies;
public class MapXLaneResultStrategyTests
{
    private readonly MapXLaneResultStrategy strategy;

    public MapXLaneResultStrategyTests()
    {
        strategy = new MapXLaneResultStrategy();
    }

    [Theory]
    [InlineData("T", "T")]
    [InlineData("NT", "MB")]
    [InlineData("M", "M")]
    [InlineData("NM", "TB")]
    [InlineData("B", "Bottom")]
    [InlineData("NB", "TM")]
    [InlineData("O", "O")]
    [InlineData("N", "None")]
    [InlineData("X", "")]
    public void Generate_ShouldReturnCorrectResults_WhenInputsAreValid(string input, string expected)
    {
        // Arrange
        var resultChanged = new ResultChanged
        {
            ResourceId = "ABC123",
            HomeValue = input,
        };

        var resourceItem = new ResourceItem
        {
            ResourceId = "ABC123",
            HomeValue = input,
        };

        // Act
        var (wrongResult, actualResult) = strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.NotNull(wrongResult);
        Assert.Equal("ABC123", wrongResult.ResourceId);
        Assert.Equal(expected, wrongResult.Home);
        Assert.Equal("ABC1", wrongResult.RefBetTypeId);
        Assert.Equal("23", wrongResult.SetX);

        Assert.NotNull(actualResult);
        Assert.Equal("ABC123", actualResult.ResourceId);
        Assert.Equal(expected, actualResult.Home);
        Assert.Equal("ABC1", actualResult.RefBetTypeId);
        Assert.Equal("23", actualResult.SetX);
    }
}
