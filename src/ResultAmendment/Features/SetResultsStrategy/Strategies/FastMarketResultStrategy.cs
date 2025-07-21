using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Models;

namespace ResultAmendment.Features.SetResultsStrategy.Strategies;

public class FastMarketResultStrategy : ISetResultStrategy
{
    public (ResultItem wrongResult, ResultItem actualResult) Generate(ResultChanged resultChanged, ResourceItem resourceItem)
    {
        var wrongResult = new ResultItem
        {
            ResourceId = resultChanged.ResourceId,
            Position = resultChanged.OtherInfo.Split('|')[0],
            Home = resultChanged.HomeValue,
            Away = resultChanged.AwayValue,
            Action = resultChanged.Action,
        };

        var actualResult = new ResultItem
        {
            ResourceId = resourceItem.ResourceId,
            Position = resourceItem.OtherInfo.Split('|')[0],
            Home = resourceItem.HomeValue,
            Away = resourceItem.AwayValue,
            Action = resourceItem.Action,
        };

        return (wrongResult, actualResult);
    }
}