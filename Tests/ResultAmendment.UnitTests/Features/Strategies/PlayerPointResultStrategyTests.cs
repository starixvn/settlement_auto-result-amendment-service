using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Features.SetResultsStrategy.Strategies;

namespace ResultAmendment.UnitTests.Features.Strategies;

public class PlayerPointResultStrategyTests
{
    private readonly PlayerPointResultStrategy _strategy = new();

    private static ResultChanged CreateResultChanged(string resourceId = "68011", string home = "h", string away = "a")
        => new()
        {
            ResourceId = resourceId,
            HomeValue = home,
            AwayValue = away
        };

    [Fact]
    public void Generate_ShouldReturnCorrectWrongAndActualResults()
    {
        // Arrange
        var resultChanged = CreateResultChanged(resourceId: "68011", home: "l", away: "m");
        var resourceItem = new ResourceItem
        {
            ResourceId = "68011",
            HomeValue = "n",
            AwayValue = "o",
            OtherInfo = "{\"pid\":777}"
        };

        // Act
        var (wrongResult, actualResult) = _strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.Equal("68011", wrongResult.ResourceId);
        Assert.Equal("68011", actualResult.ResourceId);

        // betTypeId should strip last 2 chars => 680
        Assert.Equal("680", wrongResult.RefBetTypeId);
        Assert.Equal("680", actualResult.RefBetTypeId);

        // Home/Away values uppercased by ResultItem setters
        Assert.Equal("L", wrongResult.Home);
        Assert.Equal("M", wrongResult.Away);
        Assert.Equal("N", actualResult.Home);
        Assert.Equal("O", actualResult.Away);

        // OtherInfo set to PlayerId
        Assert.Equal("777", wrongResult.OtherInfo);
        Assert.Equal("777", actualResult.OtherInfo);
    }

    [Fact]
    public void Generate_WhenOtherInfoIsInvalidJson_ShouldThrow()
    {
        // Arrange
        var resultChanged = CreateResultChanged();
        var resourceItem = new ResourceItem
        {
            ResourceId = resultChanged.ResourceId,
            HomeValue = "h",
            AwayValue = "a",
            OtherInfo = "not-json"
        };

        // Act & Assert
        Assert.ThrowsAny<Exception>(() => _strategy.Generate(resultChanged, resourceItem));
    }

    [Fact]
    public void Generate_WhenHomeAwayLowerCase_ShouldUpperCaseInResults()
    {
        // Arrange
        var resultChanged = CreateResultChanged(home: "h", away: "a");
        var resourceItem = new ResourceItem
        {
            ResourceId = resultChanged.ResourceId,
            HomeValue = "x",
            AwayValue = "y",
            OtherInfo = "{\"pid\":555}"
        };

        // Act
        var (wrongResult, actualResult) = _strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.Equal("H", wrongResult.Home);
        Assert.Equal("A", wrongResult.Away);
        Assert.Equal("X", actualResult.Home);
        Assert.Equal("Y", actualResult.Away);
    }
}
