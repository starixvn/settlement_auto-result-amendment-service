using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Features.SetResultsStrategy.Strategies;

namespace ResultAmendment.UnitTests.Features.Strategies;

public class PlayerMostPointsResultStrategyTests
{
    private readonly PlayerMostPointsResultStrategy _strategy = new();

    private static ResultChanged CreateResultChanged(string resourceId = "68011", string home = "h") => new()
    {
        ResourceId = resourceId,
        HomeValue = home,
        AwayValue = "a"
    };

    private static ResourceItem CreateResourceItem(string resourceId = "68011", string home = "x") => new()
    {
        ResourceId = resourceId,
        HomeValue = home,
        AwayValue = "y",
        OtherInfo = "[]", // currently ignored by strategy,
        OddsInfo = []
    };

    [Fact]
    public void Generate_ShouldProduceExpectedWrongAndActualResults()
    {
        // Arrange
        var resultChanged = CreateResultChanged();
        var resourceItem = CreateResourceItem();

        // Act
        var (wrong, actual) = _strategy.Generate(resultChanged, resourceItem);

        // Assert ResourceId
        Assert.Equal(resultChanged.ResourceId, wrong.ResourceId);
        Assert.Equal(resourceItem.ResourceId, actual.ResourceId);

        // betTypeId = resourceId without last 2 chars (length 5 -> first 3 chars)
        Assert.Equal("680", wrong.RefBetTypeId);
        Assert.Equal("680", actual.RefBetTypeId);

        // Home values uppercase via setter
        Assert.Equal("H", wrong.Home);
        Assert.Equal("X", actual.Home);

        // Away not set in strategy -> remains null
        Assert.Null(wrong.Away);
        Assert.Null(actual.Away);

        // TeamIds empty because internal odds list is empty
        Assert.Equal(string.Empty, wrong.TeamIds);
        Assert.Equal(string.Empty, actual.TeamIds);

        // OtherInfo built from empty odds list => empty string
        Assert.Equal(string.Empty, wrong.OtherInfo);
        Assert.Equal(string.Empty, actual.OtherInfo);
    }

    [Fact]
    public void Generate_WithDifferentResourceId_ShouldSliceBetTypeIdCorrectly()
    {
        // Arrange
        var resultChanged = CreateResultChanged(resourceId: "68099", home: "l");
        var resourceItem = CreateResourceItem(resourceId: "68099", home: "m");

        // Act
        var (wrong, actual) = _strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.Equal("680", wrong.RefBetTypeId);
        Assert.Equal("L", wrong.Home);
        Assert.Equal("M", actual.Home);
    }
}
