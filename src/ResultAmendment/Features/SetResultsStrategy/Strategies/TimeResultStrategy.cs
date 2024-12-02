using ResultAmendment.Models;

namespace ResultAmendment.Features.SetResultsStrategy.Strategies;

public class TimeResultStrategy : ISetResultStrategy // Es : E6
{
    public (ResultItem wrongResult, ResultItem actualResult) Generate(ResultChanged resultChanged, ResourceItem resourceItem)
    {
        var resourceId = resultChanged.ResourceId;
        var betTypeId = resultChanged.ResourceId[..^2];
        var mapX = Convert.ToInt16(resourceId[^2..^0]);
        var wrongResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = ResultInfoHelpers.FormatTimeValue(resultChanged.HomeValue),
            Away = ResultInfoHelpers.FormatTimeValue(resultChanged.AwayValue),
            RefBetTypeId = betTypeId,
            SetX = $"{mapX}",
            GroupBetType = $"{resourceItem.GroupBetType}",
        };

        var actualResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = ResultInfoHelpers.FormatTimeValue(resourceItem.HomeValue),
            Away = ResultInfoHelpers.FormatTimeValue(resourceItem.AwayValue),
            RefBetTypeId = betTypeId,
            SetX = $"{mapX}",
            GroupBetType = $"{resourceItem.GroupBetType}",
        };

        return (wrongResult, actualResult);
    }
}

