using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Features.SetResultsStrategy.Strategies;

namespace ResultAmendment.UnitTests.Features.Strategies;

public class PlayerDoubleResultStrategyTests
{
    private readonly PlayerDoubleResultStrategy _strategy = new();

    private static ResultChanged CreateResultChanged(string resourceId = "67911", string home = "h", string away = "a") => new()
    {
        ResourceId = resourceId,
        HomeValue = home,
        AwayValue = away
    };

    private static ResourceItem CreateResourceItem(string resourceId = "67911", string home = "x", string away = "y", int playerId = 123) => new()
    {
        ResourceId = resourceId,
        HomeValue = home,
        AwayValue = away,
        OtherInfo = $"{{\"pid\":{playerId}}}" // JSON used for deserialization
    };

    [Fact]
    public void Generate_ShouldReturnExpectedWrongAndActualResults()
    {
        // Arrange
        var resultChanged = CreateResultChanged(resourceId: "67723", home: "l", away: "m");
        var resourceItem = CreateResourceItem(resourceId: "67723", home: "n", away: "o", playerId: 777);

        // Act
        var (wrong, actual) = _strategy.Generate(resultChanged, resourceItem);

        // Assert ResourceId
        Assert.Equal("67723", wrong.ResourceId);
        Assert.Equal("67723", actual.ResourceId);

        // betTypeId removes last 2 chars => 677
        Assert.Equal("677", wrong.RefBetTypeId);
        Assert.Equal("677", actual.RefBetTypeId);

        // Home/Away casing
        Assert.Equal("L", wrong.Home);
        Assert.Equal("N", actual.Home);

    }

    [Fact]
    public void Generate_InvalidJsonInOtherInfo_ShouldThrow()
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
    public void Generate_LowerCaseValues_ShouldUppercase()
    {
        // Arrange
        var resultChanged = CreateResultChanged(home: "h", away: "a");
        var resourceItem = CreateResourceItem(home: "x", away: "y", playerId: 555);

        // Act
        var (wrong, actual) = _strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.Equal("H", wrong.Home);
        Assert.Equal("X", actual.Home);
        Assert.Equal("555", wrong.OtherInfo);
    }
}
