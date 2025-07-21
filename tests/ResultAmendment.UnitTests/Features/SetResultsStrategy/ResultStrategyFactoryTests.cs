using ResultAmendment.Features.SetResultsStrategy;
using ResultAmendment.Features.SetResultsStrategy.Strategies;

namespace ResultAmendment.UnitTests.Features.SetResultsStrategy;

public class ResultStrategyFactoryTests
{
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

    [Fact]
    public void GetStrategy_ShouldReturnMainResultStrategy_WhenResourceIdInMainResource_WithMultipleResources()
    {
        // Arrange
        var resourceId = "Q1";
        var groupBetType = 0;
        var mainResource = new List<string> { "Q1", "Q2", "Q3" };

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<MainResultStrategy>(strategy);
    }

    [Fact]
    public void GetStrategy_ShouldReturnTeamResultStrategy_WhenResourceIdIs604()
    {
        // Arrange
        var resourceId = "604";
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<TeamResultStrategy>(strategy);
    }

    [Fact]
    public void GetStrategy_ShouldReturnTeamResultStrategy_WhenResourceIdIs605()
    {
        // Arrange
        var resourceId = "605";
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<TeamResultStrategy>(strategy);
    }

    [Fact]
    public void GetStrategy_ShouldReturnTeamResultStrategy_WhenResourceIdIs3901()
    {
        // Arrange
        var resourceId = "3901";
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<TeamResultStrategy>(strategy);
    }

    [Fact]
    public void GetStrategy_ShouldReturnTeamXXResultStrategy_WhenResourceIdStartsWith606()
    {
        // Arrange
        var resourceId = "606123";
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<TeamXXResultStrategy>(strategy);
    }

    [Fact]
    public void GetStrategy_ShouldReturnTeamXXResultStrategy_WhenResourceIdStartsWith607()
    {
        // Arrange
        var resourceId = "607456";
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<TeamXXResultStrategy>(strategy);
    }

    [Fact]
    public void GetStrategy_ShouldReturnTeamXXResultStrategy_WhenResourceIdStartsWith617()
    {
        // Arrange
        var resourceId = "617789";
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<TeamXXResultStrategy>(strategy);
    }

    [Fact]
    public void GetStrategy_ShouldReturnTeamXXYYResultStrategy_WhenResourceIdStartsWith613()
    {
        // Arrange
        var resourceId = "613123";
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<TeamXXYYResultStrategy>(strategy);
    }

    [Fact]
    public void GetStrategy_ShouldReturnTeamXXYYResultStrategy_WhenResourceIdStartsWith712()
    {
        // Arrange
        var resourceId = "712456";
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<TeamXXYYResultStrategy>(strategy);
    }

    [Fact]
    public void GetStrategy_ShouldReturnTeamXXYYResultStrategy_WhenResourceIdStartsWith713()
    {
        // Arrange
        var resourceId = "713789";
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<TeamXXYYResultStrategy>(strategy);
    }

    #region Length 6 Resource Tests

    [Theory]
    [InlineData("911300")]
    [InlineData("911400")]
    [InlineData("913000")]
    [InlineData("913500")]
    [InlineData("913800")]
    public void GetStrategy_ShouldReturnMapXLaneResultStrategy_WhenResourceIdMatchesLaneResultPattern(string resourceId)
    {
        // Arrange
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<MapXLaneResultStrategy>(strategy);
    }

    [Theory]
    [InlineData("912400")]
    [InlineData("913200")]
    [InlineData("913300")]
    [InlineData("907500")]
    [InlineData("908700")]
    public void GetStrategy_ShouldReturnMapXYesNoResultStrategy_WhenResourceIdMatchesYesNoPattern(string resourceId)
    {
        // Arrange
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<MapXYesNoResultStrategy>(strategy);
    }

    [Theory]
    [InlineData("914100")]
    [InlineData("914200")]
    public void GetStrategy_ShouldReturnMapXOverlordTyrantResultStrategy_WhenResourceIdStartsWith9141Or9142(string resourceId)
    {
        // Arrange
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<MapXOverlordTyrantResultStrategy>(strategy);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void GetStrategy_ShouldReturnTeamXXResultStrategy_WhenGroupBetTypeIsTeamType(int groupBetType)
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
    public void GetStrategy_ShouldReturnTimeResultStrategy_WhenGroupBetTypeIsDuration()
    {
        // Arrange
        var resourceId = "123456";
        var groupBetType = 5; // Duration
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<TimeResultStrategy>(strategy);
    }

    [Fact]
    public void GetStrategy_ShouldReturnMapXResultStrategy_WhenLength6AndNoSpecialConditions()
    {
        // Arrange
        var resourceId = "123456";
        var groupBetType = 4; // Non-team, non-duration
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<MapXResultStrategy>(strategy);
    }

    #endregion Length 6 Resource Tests

    #region Length 8 Resource Tests

    [Fact]
    public void GetStrategy_ShouldReturnTeamXYOrdinalResultStrategy_WhenResourceIdStartsWith9101()
    {
        // Arrange
        var resourceId = "91011234";
        var groupBetType = 0;
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
        var resourceId = "39031234";
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
        var resourceId = "39151234";
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
    public void GetStrategy_ShouldReturnTeamXXYYResultStrategy_WhenLength8AndGroupBetTypeIsTeamType(int groupBetType)
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
    public void GetStrategy_ShouldReturnTimeKillsResultStrategy_WhenLength8AndGroupBetTypeIsDuration()
    {
        // Arrange
        var resourceId = "12345678";
        var groupBetType = 5; // Duration
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<TimeKillsResultStrategy>(strategy);
    }

    [Fact]
    public void GetStrategy_ShouldReturnMapXYResultStrategy_WhenLength8AndNoSpecialConditions()
    {
        // Arrange
        var resourceId = "12345678";
        var groupBetType = 4; // Non-team, non-duration
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<MapXYResultStrategy>(strategy);
    }

    #endregion Length 8 Resource Tests

    #region Edge Cases and Default Tests

    [Fact]
    public void GetStrategy_ShouldReturnDefaultMainResultStrategy_WhenResourceIdDoesNotMatch()
    {
        // Arrange
        var resourceId = "unknown123";
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<MainResultStrategy>(strategy);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("123")]
    [InlineData("1234")]
    [InlineData("12345")]
    [InlineData("1234567")]
    [InlineData("123456789")]
    public void GetStrategy_ShouldReturnMainResultStrategy_WhenResourceIdLengthIsNot6Or8(string resourceId)
    {
        // Arrange
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<MainResultStrategy>(strategy);
    }

    #endregion Edge Cases and Default Tests

    #region GetPositionResultStrategy Tests

    [Fact]
    public void GetPositionResultStrategy_ShouldReturnFastMarketResultStrategy()
    {
        // Arrange
        var resourceId = "221";

        // Act
        var strategy = ResultStrategyFactory.GetPositionResultStrategy(resourceId);

        // Assert
        Assert.IsType<FastMarketResultStrategy>(strategy);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("999999")]
    [InlineData(null)]
    public void GetPositionResultStrategy_ShouldReturnFastMarketResultStrategy_ForAnyResourceId(string resourceId)
    {
        // Act
        var strategy = ResultStrategyFactory.GetPositionResultStrategy(resourceId);

        // Assert
        Assert.IsType<FastMarketResultStrategy>(strategy);
    }

    #endregion GetPositionResultStrategy Tests
}