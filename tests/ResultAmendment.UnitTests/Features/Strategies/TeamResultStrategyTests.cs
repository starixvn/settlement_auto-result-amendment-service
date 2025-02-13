using ResultAmendment.Features.SetResultsStrategy.Strategies;
using ResultAmendment.Models;

namespace ResultAmendment.UnitTests.Features.Strategies;

public class TeamResultStrategyTests
{
    private readonly TeamResultStrategy strategy;

    public TeamResultStrategyTests()
    {
        strategy = new TeamResultStrategy();
    }

    [Fact]
    public void Generate_ShouldReturnCorrectResults_WhenInputsAreValid()
    {
        // Arrange
        var resultChanged = new ResultChanged
        {
            ResourceId = "12345",
            HomeValue = "H"
        };

        var resourceItem = new ResourceItem
        {
            ResourceId = "12345",
            HomeValue = "A",
            AwayId = "1111",
            HomeId = "2222"
        };

        // Act
        var (wrongResult, actualResult) = strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.Equal("12345", wrongResult.ResourceId);
        Assert.Equal("H", wrongResult.Home);
        Assert.Equal("A", actualResult.Home);
        Assert.Equal("", wrongResult.Away);
        Assert.Equal("", actualResult.Away);
        Assert.Equal("12345", wrongResult.RefBetTypeId);
        Assert.Equal("2222", wrongResult.TeamIds);
        Assert.Equal("1111", actualResult.TeamIds);
    }

    [Fact]
    public void Generate_ShouldHandleEmptyHomeValue()
    {
        // Arrange
        var resultChanged = new ResultChanged
        {
            ResourceId = "12345",
            HomeValue = ""
        };

        var resourceItem = new ResourceItem
        {
            ResourceId = "67890",
            HomeValue = ""
        };

        // Act
        var (wrongResult, actualResult) = strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.Equal("", wrongResult.Home);
        Assert.Equal("", actualResult.Home);
        Assert.Empty(wrongResult.TeamIds);
        Assert.Empty(actualResult.TeamIds);
    }
}
