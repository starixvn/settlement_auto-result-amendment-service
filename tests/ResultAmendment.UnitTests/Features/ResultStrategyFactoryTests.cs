using ResultAmendment.Features.SetResultsStrategy;
using ResultAmendment.Features.SetResultsStrategy.Strategies;

namespace ResultAmendment.UnitTests.Features;

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
    public void GetStrategy_ShouldReturnMapXLaneResultStrategy_WhenResourceIdMatchesLaneResultPattern()
    {
        // Arrange
        var resourceId = "911300";
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<MapXLaneResultStrategy>(strategy);
    }

    [Fact]
    public void GetStrategy_ShouldReturnMapXYesNoResultStrategy_WhenResourceIdMatchesYesNoPattern()
    {
        // Arrange
        var resourceId = "912412";
        var groupBetType = 0;
        var mainResource = new List<string>();

        // Act
        var strategy = ResultStrategyFactory.GetStrategy(resourceId, groupBetType, mainResource);

        // Assert
        Assert.IsType<MapXYesNoResultStrategy>(strategy);
    }

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
}
