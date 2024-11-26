using ResultAmendment.Models;

namespace ResultAmendment.Features.SetResultsStrategy.Strategies;
public class MainResultStrategy : ISetResultStrategy
{
    public (ResultItem wrongResult, ResultItem actualResult) Generate(ResultChanged resultChanged, ResourceItem resourceItem)
    {
        var wrongResult = new ResultItem
        {
            ResourceId = resultChanged.ResourceId,
            Home = resultChanged.HomeValue,
            Away = resultChanged.AwayValue,
            RefBetTypeId = resultChanged.ResourceId,
        };

        var actualResult = new ResultItem
        {
            ResourceId = resourceItem.ResourceId,
            Home = resourceItem.HomeValue,
            Away = resourceItem.AwayValue,
            RefBetTypeId = resourceItem.ResourceId,
        };

        return (wrongResult, actualResult);
    }
}
