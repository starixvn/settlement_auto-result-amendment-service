using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Features.SetResultsStrategy;

namespace ResultAmendment.UnitTests.Features.SetResultsStrategy;

public class ResultItemHelperTests
{
    [Theory]
    [InlineData("ResourceId", "BetTypeId", 1, 2, true, "K2")]
    [InlineData("ResourceId", "BetTypeId", 1, 2, false, null)]
    public void CreateResultItem_WithResultChanged_ShouldReturnCorrectResultItem(string resourceId, string betTypeId, int setX, int pointY, bool includeOrdinal, string expectedOrdinal)
    {
        // Arrange
        var resultChanged = new ResultChanged
        {
            HomeValue = "h",
            AwayValue = "a"
        };
        var resourceItem = new ResourceItem();

        // Act
        var result = ResultItemHelper.CreateResultItem(resultChanged, resourceItem, resourceId, betTypeId, setX, pointY, includeOrdinal);

        // Assert
        Assert.Equal(resourceId, result.ResourceId);
        Assert.Equal("H", result.Home);
        Assert.Equal("A", result.Away);
        Assert.Equal(betTypeId, result.RefBetTypeId);
        Assert.Equal("1", result.SetX);
        Assert.Equal("2", result.PointY);
        Assert.Equal(expectedOrdinal, result.Ordinal);
    }

    [Theory]
    [InlineData("ResourceId", "BetTypeId", 1, 2, true, "K2")]
    [InlineData("ResourceId", "BetTypeId", 1, 2, false, null)]
    public void CreateResultItem_WithResourceItem_ShouldReturnCorrectResultItem(string resourceId, string betTypeId, int setX, int pointY, bool includeOrdinal, string? expectedOrdinal)
    {
        // Arrange
        var resourceItem = new ResourceItem
        {
            HomeValue = "h",
            AwayValue = "a"
        };

        // Act
        var result = ResultItemHelper.CreateResultItem(resourceItem, resourceId, betTypeId, setX, pointY, includeOrdinal);

        // Assert
        Assert.Equal(resourceId, result.ResourceId);
        Assert.Equal("H", result.Home);
        Assert.Equal("A", result.Away);
        Assert.Equal(betTypeId, result.RefBetTypeId);
        Assert.Equal("1", result.SetX);
        Assert.Equal("2", result.PointY);
        Assert.Equal(expectedOrdinal, result.Ordinal);
    }
}