using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Models;

namespace ResultAmendment.Features.SetResultsStrategy.Strategies;

public class MapXYesNoResultStrategy : ISetResultStrategy
{
    public (ResultItem wrongResult, ResultItem actualResult) Generate(ResultChanged resultChanged, ResourceItem resourceItem)
    {
        var resourceId = resultChanged.ResourceId;
        var betTypeId = resultChanged.ResourceId[..^2];
        var mapX = Convert.ToInt16(resourceId[^2..^0]);
        var wrongResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = NormalizeValue(resultChanged.HomeValue),
            Away = resultChanged.AwayValue,
            RefBetTypeId = betTypeId,
            SetX = $"{mapX}",
        };

        var actualResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = NormalizeValue(resourceItem.HomeValue),
            Away = resourceItem.AwayValue,
            RefBetTypeId = betTypeId,
            SetX = $"{mapX}",
        };

        return (wrongResult, actualResult);
    }

    private static string NormalizeValue(string homeValue)
    {
        return homeValue switch
        {
            "Y" => "Y", // Yes
            "N" => "N", // No
            "NONE" => "None", // No
            _ => string.Empty
        };
    }
}
