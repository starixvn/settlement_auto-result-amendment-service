using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Models;

namespace ResultAmendment.Features.SetResultsStrategy.Strategies;

public class MapXYResultStrategy : ISetResultStrategy // Es : E4
{
    public (ResultItem wrongResult, ResultItem actualResult) Generate(ResultChanged resultChanged, ResourceItem resourceItem)
    {
        var resourceId = resultChanged.ResourceId; // 9068XXYY, 9069XXYY
        var betTypeId = resultChanged.ResourceId[..^4];
        var pointY = Convert.ToInt16(resourceId[^2..^0]);
        var setX = Convert.ToInt16(resourceId[^4..^2]);// set, map , game
        var wrongResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = resultChanged.HomeValue ?? "",
            Away = resultChanged.AwayValue ?? "",
            RefBetTypeId = betTypeId,
            SetX = $"{setX}",
            PointY = $"{pointY}",
        };

        var actualResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = resourceItem.HomeValue ?? "",
            Away = resourceItem.AwayValue ?? "",
            RefBetTypeId = betTypeId,
            SetX = $"{setX}",
            PointY = $"{pointY}",
        };

        return (wrongResult, actualResult);
    }
}
