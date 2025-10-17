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
            Home = resultChanged.HomeValue ?? string.Empty,
            Away = resultChanged.AwayValue ?? string.Empty,
            Action = resultChanged.Action,
            MarketId = resultChanged.OtherInfo ?? string.Empty
        };

        var actualResult = new ResultItem
        {
            ResourceId = resourceItem.ResourceId,
            Home = resourceItem.HomeValue ?? string.Empty,
            Away = resourceItem.AwayValue ?? string.Empty,
            Action = resourceItem.Action,
            MarketId = resourceItem.OtherInfo ?? string.Empty

        };

        return (wrongResult, actualResult);
    }
}
