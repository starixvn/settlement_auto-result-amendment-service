using ResultAmendment.Features.SetResultsStrategy;
using ResultAmendment.Features.SetResultsStrategy.Strategies;

namespace ResultAmendment.UnitTests.Features;

public class ResultStrategyFactoryTests
{
    [Fact]
    public void GetPositionResultStrategy_ShouldReturnFastMarketResultStrategy()
    {
        // Arrange
        var resourceId = "any_id";

        // Act
        var strategy = ResultStrategyFactory.GetPositionResultStrategy(resourceId);

        // Assert
        Assert.IsType<FastMarketResultStrategy>(strategy);
    }

    [Fact]
    public void GetBetBuilderResultStrategy_ShouldReturnBetBuilderResultStrategy()
    {
        // Act
        var strategy = ResultStrategyFactory.GetBetBuilderResultStrategy();

        // Assert
        Assert.IsType<BetBuilderResultStrategy>(strategy);
    }

    [Fact]
    public void GetStrategy_ShouldReturnMainResultStrategy_WhenResourceIdInMainResource()
    {
        // Arrange
        var resourceId = "00";
        var groupBetType = 0;
        var mainResource = new List<string> { "00" };

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<MainResultStrategy>(strategy);
    }

    [Theory]
    [InlineData("604")]
    [InlineData("605")]
    [InlineData("3901")]
    public void GetStrategy_ShouldReturnTeamResultStrategy_WhenResourceIdIsTeamResult(string resourceId)
    {
        // Arrange
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<TeamResultStrategy>(strategy);
    }

    [Theory]
    [InlineData("606123")]
    [InlineData("607123")]
    [InlineData("617123")]
    public void GetStrategy_ShouldReturnTeamXXResultStrategy_WhenResourceIdIsTeamXXResult(string resourceId)
    {
        // Arrange
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<TeamXXResultStrategy>(strategy);
    }

    [Theory]
    [InlineData("613123")]
    [InlineData("712123")]
    [InlineData("713123")]
    public void GetStrategy_ShouldReturnTeamXXYYResultStrategy_WhenResourceIdIsTeamXXYYResult(string resourceId)
    {
        // Arrange
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<TeamXXYYResultStrategy>(strategy);
    }

    [Theory]
    [InlineData("911300")]
    [InlineData("911400")]
    [InlineData("913000")]
    [InlineData("913500")]
    [InlineData("913800")]
    public void GetStrategy_ShouldReturnMapXLaneResultStrategy_WhenResourceIdMatchesLaneResultPattern(string resourceId)
    {
        // Arrange
        var groupBetType = 6;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<MapXLaneResultStrategy>(strategy);
    }

    [Theory]
    [InlineData("914100")]
    [InlineData("914200")]
    public void GetStrategy_ShouldReturnMapXOverlordTyrantResultStrategy_WhenResourceIdLengthIs6(string resourceId)
    {
        // Arrange
        var groupBetType = 11;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<MapXOverlordTyrantResultStrategy>(strategy);
    }

    [Theory]
    [InlineData("912412")]
    [InlineData("913212")]
    [InlineData("913312")]
    [InlineData("907512")]
    [InlineData("908712")]
    public void GetStrategy_ShouldReturnMapXYesNoResultStrategy_WhenResourceIdMatchesYesNoPattern(string resourceId)
    {
        // Arrange
        var groupBetType = 3;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<MapXYesNoResultStrategy>(strategy);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void GetStrategy_ShouldReturnTeamXXResultStrategy_WhenGroupBetTypeIsTeamAndLengthIs6(int groupBetType)
    {
        // Arrange
        var resourceId = "123456";
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<TeamXXResultStrategy>(strategy);
    }

    [Fact]
    public void GetStrategy_ShouldReturnTimeResultStrategy_WhenGroupBetTypeIsDurationAndLengthIs6()
    {
        // Arrange
        var resourceId = "123456";
        var groupBetType = 5;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<TimeResultStrategy>(strategy);
    }

    [Fact]
    public void GetStrategy_ShouldReturnMapXResultStrategy_AsDefaultForLength6()
    {
        // Arrange
        var resourceId = "123456";
        var groupBetType = 99;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<MapXResultStrategy>(strategy);
    }

    [Fact]
    public void GetStrategy_ShouldReturnTeamXYOrdinalResultStrategy_WhenResourceIdStartsWith9101()
    {
        // Arrange
        var resourceId = "91010000";
        var groupBetType = 6;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<TeamXYOrdinalResultStrategy>(strategy);
    }

    [Fact]
    public void GetStrategy_ShouldReturnSetRacePointStrategy_WhenResourceIdStartsWith3903()
    {
        // Arrange
        var resourceId = "39030000";
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<SetRacePointStrategy>(strategy);
    }

    [Fact]
    public void GetStrategy_ShouldReturnSetPointPlayedStrategy_WhenResourceIdStartsWith3915()
    {
        // Arrange
        var resourceId = "39150000";
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<SetPointPlayedStrategy>(strategy);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void GetStrategy_ShouldReturnTeamXXYYResultStrategy_WhenGroupBetTypeIsTeamAndLengthIs8(int groupBetType)
    {
        // Arrange
        var resourceId = "12345678";
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<TeamXXYYResultStrategy>(strategy);
    }

    [Fact]
    public void GetStrategy_ShouldReturnTimeKillsResultStrategy_WhenGroupBetTypeIsDurationAndLengthIs8()
    {
        // Arrange
        var resourceId = "12345678";
        var groupBetType = 5;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<TimeKillsResultStrategy>(strategy);
    }

    [Fact]
    public void GetStrategy_ShouldReturnMapXYResultStrategy_AsDefaultForLength8()
    {
        // Arrange
        var resourceId = "12345678";
        var groupBetType = 99;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<MapXYResultStrategy>(strategy);
    }

    [Fact]
    public void GetStrategy_ShouldReturnDefaultMainResultStrategy_WhenResourceIdDoesNotMatch()
    {
        // Arrange
        var resourceId = "unknown";
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<MainResultStrategy>(strategy);
    }
}