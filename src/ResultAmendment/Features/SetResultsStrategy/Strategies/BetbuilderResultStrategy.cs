using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Models;

namespace ResultAmendment.Features.SetResultsStrategy.Strategies;

public class BetBuilderResultStrategy : ISetResultStrategy
{
    public (ResultItem wrongResult, ResultItem actualResult) Generate(ResultChanged resultChanged, ResourceItem resourceItem)
    {
        var wrongResult = new ResultItem
        {
            ResourceId = resultChanged.ResourceId,
            Home = resultChanged.HomeValue,
            Away = resultChanged.AwayValue,
            Action = resultChanged.Action,
            MarketId = resultChanged.OtherInfo
        };

        var actualResult = new ResultItem
        {
            ResourceId = resourceItem.ResourceId,
            Home = resourceItem.HomeValue,
            Away = resourceItem.AwayValue,
            Action = resourceItem.Action,
            MarketId = resourceItem.OtherInfo
        };

        return (wrongResult, actualResult);
    }
}