using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Models;

namespace ResultAmendment.Features.SetResultsStrategy.Strategies;

public class TeamXXYYResultStrategy : ISetResultStrategy // BasketBall: E3, Es : E3, E5
{
    public (ResultItem wrongResult, ResultItem actualResult) Generate(ResultChanged resultChanged, ResourceItem resourceItem)
    {
        var resourceId = resultChanged.ResourceId; //613XXYY, 9068XXYY, 9069XXYY
        var betTypeId = resultChanged.ResourceId[..^4];

        var setX = Convert.ToInt16(resourceId[^4..^2]);// set, map , game
        var pointY = Convert.ToInt16(resourceId[^2..^0]);
        var wrongResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = ResultInfoHelpers.NormalizeHomeValue(resultChanged.HomeValue),
            Away = resultChanged.AwayValue ?? "",
            RefBetTypeId = betTypeId,
            TeamIds = ResultInfoHelpers.GetTeamIds(resultChanged.HomeValue, resourceItem),
            SetX = $"{setX}",
            PointY = $"{pointY}",
        };

        var actualResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = ResultInfoHelpers.NormalizeHomeValue(resourceItem.HomeValue),
            Away = resourceItem.AwayValue ?? "",
            RefBetTypeId = betTypeId,
            TeamIds = ResultInfoHelpers.GetTeamIds(resourceItem.HomeValue, resourceItem),
            SetX = $"{setX}",
            PointY = $"{pointY}",
        };

        return (wrongResult, actualResult);
    }
}
