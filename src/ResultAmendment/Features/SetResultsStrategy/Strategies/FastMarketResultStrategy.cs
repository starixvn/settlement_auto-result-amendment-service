using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Features.SetResultsStrategy;
using ResultAmendment.Models;
using System.Diagnostics.CodeAnalysis;

public class FastMarketResultStrategy : ISetResultStrategy
{
    [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
    public (ResultItem wrongResult, ResultItem actualResult) Generate(ResultChanged resultChanged, ResourceItem resourceItem)
    {
        var wrongResult = new ResultItem
        {
            ResourceId = resultChanged.ResourceId,
            Position = resultChanged.OtherInfo.Split('|').First() ?? string.Empty,
            Home = resultChanged.HomeValue ?? string.Empty,
            Away = resultChanged.AwayValue ?? string.Empty
        };

        var actualResult = new ResultItem
        {
            ResourceId = resourceItem.ResourceId,
            Position = resourceItem.OtherInfo.Split('|').First() ?? string.Empty,
            Home = resourceItem.HomeValue ?? string.Empty,
            Away = resourceItem.AwayValue ?? string.Empty
        };

        return (wrongResult, actualResult);
    }
}
