using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Models;

namespace ResultAmendment.Features.SetResultsStrategy.Strategies;

public class SetRacePointStrategy : ISetResultStrategy
{
    public (ResultItem wrongResult, ResultItem actualResult) Generate(ResultChanged resultChanged, ResourceItem resourceItem)
    {
        var resourceId = resultChanged.ResourceId; // 3903XXYY
        var betTypeId = resourceId[..^4];

        var setX = Convert.ToInt16(resourceId[^4..^2]); // set, map, game
        var pointY = Convert.ToInt16(resourceId[^2..^0]);

        var wrongResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = GetHomeValue(resultChanged.HomeValue, resultChanged.AwayValue, pointY),
            Away = "",
            RefBetTypeId = betTypeId,
            TeamIds = ResultInfoHelpers.GetTeamIds(GetHomeValue(resultChanged.HomeValue, resultChanged.AwayValue, pointY), resourceItem),
            SetX = $"{setX}",
            PointY = $"{pointY}",
        };

        var actualResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = GetHomeValue(resourceItem.HomeValue, resourceItem.AwayValue, pointY),
            Away = "",
            RefBetTypeId = betTypeId,
            TeamIds = ResultInfoHelpers.GetTeamIds(GetHomeValue(resourceItem.HomeValue, resourceItem.AwayValue, pointY), resourceItem),
            SetX = $"{setX}",
            PointY = $"{pointY}",
        };

        return (wrongResult, actualResult);
    }

    private static string GetHomeValue(string homeValue, string awayValue, int point)
    {
        int homeScore = Convert.ToInt16(homeValue);
        int awayScore = Convert.ToInt16(awayValue);

        if (homeScore >= point && homeScore > awayScore)
        {
            return "H";
        }

        if (homeScore >= point && homeScore < awayScore)
        {
            return "A";
        }

        return "None";
    }
}
