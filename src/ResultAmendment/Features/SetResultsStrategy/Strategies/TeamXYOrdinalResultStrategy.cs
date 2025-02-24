using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Models;

namespace ResultAmendment.Features.SetResultsStrategy.Strategies;

public class TeamXYOrdinalResultStrategy : ISetResultStrategy
{
    public (ResultItem wrongResult, ResultItem actualResult) Generate(ResultChanged resultChanged, ResourceItem resourceItem)
    {
        var resourceId = resultChanged.ResourceId;
        var betTypeId = resourceId[..^4];

        var setX = Convert.ToInt16(resourceId[^4..^2]); // set, map, game
        var pointY = Convert.ToInt16(resourceId[^2..^0]);

        var wrongResult = ResultItemHelper.CreateResultItem(resultChanged, resourceItem, resourceId, betTypeId, setX, pointY, true);
        var actualResult = ResultItemHelper.CreateResultItem(resourceItem, resourceId, betTypeId, setX, pointY, true);

        return (wrongResult, actualResult);
    }
}