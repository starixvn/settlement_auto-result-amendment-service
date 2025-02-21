using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Models;

namespace ResultAmendment.Features.SetResultsStrategy.Strategies;

public class MapXResultStrategy : ISetResultStrategy
{
    public (ResultItem wrongResult, ResultItem actualResult) Generate(ResultChanged resultChanged, ResourceItem resourceItem)
    {
        var resourceId = resultChanged.ResourceId;
        var betTypeId = resultChanged.ResourceId[..^2];
        var setX = Convert.ToInt16(resourceId[^2..^0]);
        var wrongResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = resultChanged.HomeValue,
            Away = resultChanged.AwayValue ?? "",
            RefBetTypeId = betTypeId,
            SetX = $"{setX}",
        };

        var actualResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = resourceItem.HomeValue,
            Away = resourceItem.AwayValue ?? "",
            RefBetTypeId = betTypeId,
            SetX = $"{setX}",
        };

        return (wrongResult, actualResult);
    }
}
