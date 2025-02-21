using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Models;

namespace ResultAmendment.Features.SetResultsStrategy.Strategies;

public class TeamXXResultStrategy : ISetResultStrategy
{
    public (ResultItem wrongResult, ResultItem actualResult) Generate(ResultChanged resultChanged, ResourceItem resourceItem)
    {
        var resourceId = resultChanged.ResourceId;
        var betTypeId = resultChanged.ResourceId[..^2];
        var setX = Convert.ToInt16(resourceId[^2..^0]); // set, map , game or point
        var wrongResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = ResultInfoHelpers.NormalizeHomeValue(resultChanged.HomeValue),
            Away = resultChanged.AwayValue ?? "",
            RefBetTypeId = betTypeId,
            TeamIds = ResultInfoHelpers.GetTeamIds(resultChanged.HomeValue, resourceItem),
            SetX = $"{setX}",
        };

        var actualResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = ResultInfoHelpers.NormalizeHomeValue(resourceItem.HomeValue),
            Away = resourceItem.AwayValue ?? "",
            RefBetTypeId = betTypeId,
            TeamIds = ResultInfoHelpers.GetTeamIds(resourceItem.HomeValue, resourceItem),
            SetX = $"{setX}",
        };

        return (wrongResult, actualResult);
    }
}
