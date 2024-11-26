using ResultAmendment.Models;

namespace ResultAmendment.Features.SetResultsStrategy.Strategies;

public class MapXLaneResultStrategy : ISetResultStrategy
{
    public (ResultItem wrongResult, ResultItem actualResult) Generate(ResultChanged resultChanged, ResourceItem resourceItem)
    {
        var resourceId = resultChanged.ResourceId;
        var betTypeId = resultChanged.ResourceId[..^2];
        var mapX = Convert.ToInt16(resourceId[^2..^0]);
        var wrongResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = NormalizeLaneValue(resultChanged.HomeValue),
            Away = resultChanged.AwayValue ?? "",
            RefBetTypeId = betTypeId,
            SetX = $"{mapX}",
        };

        var actualResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = NormalizeLaneValue(resourceItem.HomeValue),
            Away = resourceItem.AwayValue ?? "",
            RefBetTypeId = betTypeId,
            SetX = $"{mapX}",
        };

        return (wrongResult, actualResult);
    }

    private static string NormalizeLaneValue(string homeValue)
    {
        return homeValue switch
        {
            "T" => "T", // Top
            "NT" => "MB", // Middle Bottom
            "M" => "M", // Middle
            "NM" => "TB", // Top Bottom
            "B" => "B", // Bottom
            "NB" => "TM", // Top Middle
            "O" => "O", // Overlord
            "N" => "None", // None
            _ => string.Empty
        };
    }
}
