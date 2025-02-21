using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Models;

namespace ResultAmendment.Features.SetResultsStrategy.Strategies;
public class TeamResultStrategy : ISetResultStrategy
{
    public (ResultItem wrongResult, ResultItem actualResult) Generate(ResultChanged resultChanged, ResourceItem resourceItem)
    {
        var wrongResult = new ResultItem
        {
            ResourceId = resultChanged.ResourceId,
            Home = ResultInfoHelpers.NormalizeHomeValue(resultChanged.HomeValue),
            Away = "",
            RefBetTypeId = resultChanged.ResourceId,
            TeamIds = ResultInfoHelpers.GetTeamIds(resultChanged.HomeValue, resourceItem),
        };

        var actualResult = new ResultItem
        {
            ResourceId = resourceItem.ResourceId,
            Home = ResultInfoHelpers.NormalizeHomeValue(resourceItem.HomeValue),
            Away = "",
            RefBetTypeId = resourceItem.ResourceId,
            TeamIds = ResultInfoHelpers.GetTeamIds(resourceItem.HomeValue, resourceItem),
        };

        return (wrongResult, actualResult);
    }

}