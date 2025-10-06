using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Features.SetResultsStrategy.Strategies;

namespace ResultAmendment.UnitTests.Features.Strategies;

public class PlayerPerformanceResultStrategyTests
{
    private readonly PlayerPerformanceResultStrategy _strategy = new();

    private static ResultChanged CreateResultChanged(string resourceId = "67701", string home = "h") => new()
    {
        ResourceId = resourceId,
        HomeValue = home,
    };

    private static ResourceItem CreateResourceItem(string resourceId = "67701", string home = "x", string json = "{\"PlayerId\":\"777\"}") => new()
    {
        ResourceId = resourceId,
        HomeValue = home,
        OtherInfo = json,
        OddsInfo = []
    };

    [Fact]
    public void Generate_ShouldBuildWrongAndActualResult_WithExpectedFields()
    {
        // Arrange
        var resultChanged = CreateResultChanged(resourceId: "67723", home: "l");
        var resourceItem = CreateResourceItem(resourceId: "67723", home: "m");

        // Act
        var (wrong, actual) = _strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.Equal("67723", wrong.ResourceId);
        Assert.Equal("67723", actual.ResourceId);

        // betTypeId = resourceId without last 2 chars
        Assert.Equal("677", wrong.RefBetTypeId);
        Assert.Equal("677", actual.RefBetTypeId);

        // Home values upper-cased by ResultItem setter
        Assert.Equal("L", wrong.Home);
        Assert.Equal("M", actual.Home);

        // Away not set in this strategy
        Assert.Null(wrong.Away);
        Assert.Null(actual.Away);
    }

    [Fact]
    public void Generate_WhenLowerCaseValues_ShouldUpperCaseInResult()
    {
        // Arrange
        var resultChanged = CreateResultChanged(home: "h");
        var resourceItem = CreateResourceItem(home: "a");

        // Act
        var (wrong, actual) = _strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.Equal("H", wrong.Home);
        Assert.Equal("A", actual.Home);
    }
}
