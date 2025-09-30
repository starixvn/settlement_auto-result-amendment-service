using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Models;

namespace ResultAmendment.Features.SetResultsStrategy.Strategies;

public class PlayerPerformanceResultStrategy : ISetResultStrategy // basektball: 677, 679
{
    public (ResultItem wrongResult, ResultItem actualResult) Generate(ResultChanged resultChanged, ResourceItem resourceItem)
    {
        var resourceId = resultChanged.ResourceId;
        var betTypeId = resultChanged.ResourceId[..^2];

        var wrongResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = resultChanged.HomeValue,
            RefBetTypeId = betTypeId,
            OtherInfo = resourceItem.OtherInfo
        };

        var actualResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = resourceItem.HomeValue,
            RefBetTypeId = betTypeId,
            OtherInfo = resourceItem.OtherInfo
        };

        return (wrongResult, actualResult);
    }
}
