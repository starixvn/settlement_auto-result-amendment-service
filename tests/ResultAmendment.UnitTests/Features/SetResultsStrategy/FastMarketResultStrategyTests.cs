using Fanex.Settlement.MessageBridge.ResultAmendment;

public class FastMarketResultStrategyTests
{
    [Fact]
    public void Generate_ReturnsExpectedResult_WhenOtherInfoIsPipeDelimited()
    {
        // Arrange
        var strategy = new FastMarketResultStrategy();
        var resultChanged = new ResultChanged
        {
            ResourceId = "221",
            OtherInfo = "P1|Extra",
            HomeValue = "H1",
            AwayValue = "A1"
        };
        var resourceItem = new ResourceItem
        {
            ResourceId = "222",
            OtherInfo = "P2|Extra2",
            HomeValue = "H2",
            AwayValue = "A2"
        };

        // Act
        var (wrongResult, actualResult) = strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.Equal("221", wrongResult.ResourceId);
        Assert.Equal("P1", wrongResult.Position);
        Assert.Equal("H1", wrongResult.Home);
        Assert.Equal("A1", wrongResult.Away);

        Assert.Equal("222", actualResult.ResourceId);
        Assert.Equal("P2", actualResult.Position);
        Assert.Equal("H2", actualResult.Home);
        Assert.Equal("A2", actualResult.Away);
    }

    [Fact]
    public void Generate_ReturnsEmptyPosition_WhenOtherInfoHasNoPipe()
    {
        var strategy = new FastMarketResultStrategy();
        var resultChanged = new ResultChanged
        {
            ResourceId = "RC2",
            OtherInfo = "",
            HomeValue = "H1",
            AwayValue = "A1"
        };
        var resourceItem = new ResourceItem
        {
            ResourceId = "RI2",
            OtherInfo = "",
            HomeValue = "H2",
            AwayValue = "A2"
        };

        var (wrongResult, actualResult) = strategy.Generate(resultChanged, resourceItem);

        Assert.Equal("", wrongResult.Position);
        Assert.Equal("", actualResult.Position);
    }

    [Fact]
    public void Generate_ReturnsEmptyStrings_WhenHomeOrAwayIsNull()
    {
        var strategy = new FastMarketResultStrategy();
        var resultChanged = new ResultChanged
        {
            ResourceId = "RC3",
            OtherInfo = "P3|",
        };
        var resourceItem = new ResourceItem
        {
            ResourceId = "RI3",
            OtherInfo = "P4|",
        };

        var (wrongResult, actualResult) = strategy.Generate(resultChanged, resourceItem);

        Assert.Equal("", wrongResult.Home);
        Assert.Equal("", wrongResult.Away);
        Assert.Equal("", actualResult.Home);
        Assert.Equal("", actualResult.Away);
    }
}
