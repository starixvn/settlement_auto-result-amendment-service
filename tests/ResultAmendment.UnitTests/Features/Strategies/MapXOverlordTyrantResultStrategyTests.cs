using ResultAmendment.Features.SetResultsStrategy.Strategies;
using ResultAmendment.Models;

namespace ResultAmendment.UnitTests.Features.Strategies;
public class MapXOverlordTyrantResultStrategyTests
{
    private readonly MapXOverlordTyrantResultStrategy strategy;

    public MapXOverlordTyrantResultStrategyTests()
    {
        strategy = new MapXOverlordTyrantResultStrategy();
    }

    [Theory]
    [InlineData("T", "TR")]
    [InlineData("O", "O")]
    [InlineData("NT", "OM")]
    [InlineData("N", "None")]
    [InlineData("X", "")]
    public void Generate_ShouldReturnCorrectResults_WhenInputsAreValid(string input, string expected)
    {
        // Arrange
        var resultChanged = new ResultChanged
        {
            ResourceId = "ABC112",
            HomeValue = input,
        };

        var resourceItem = new ResourceItem
        {
            ResourceId = "ABC112",
            HomeValue = input,
        };

        // Act
        var (wrongResult, actualResult) = strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.NotNull(wrongResult);
        Assert.Equal("ABC112", wrongResult.ResourceId);
        Assert.Equal(expected, wrongResult.Home);
        Assert.Equal("ABC1", wrongResult.RefBetTypeId);
        Assert.Equal("12", wrongResult.SetX);

        Assert.NotNull(actualResult);
        Assert.Equal("ABC112", actualResult.ResourceId);
        Assert.Equal(expected, actualResult.Home);
        Assert.Equal("ABC1", actualResult.RefBetTypeId);
        Assert.Equal("12", actualResult.SetX);
    }
}
