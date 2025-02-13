using ResultAmendment.Models;

namespace ResultAmendment.Features.SetResultsStrategy.Strategies;

public class MapXOverlordTyrantResultStrategy : ISetResultStrategy
{
    public (ResultItem wrongResult, ResultItem actualResult) Generate(ResultChanged resultChanged, ResourceItem resourceItem)
    {
        var resourceId = resultChanged.ResourceId;
        var betTypeId = resultChanged.ResourceId[..^2];
        var setX = Convert.ToInt16(resourceId[^2..^0]); // last character of resourceId
        var wrongResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = NormalizeHomeValue(resultChanged.HomeValue),
            Away = resultChanged.AwayValue,
            RefBetTypeId = betTypeId,
            SetX = $"{setX}",
        };

        var actualResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = NormalizeHomeValue(resourceItem.HomeValue),
            Away = resourceItem.AwayValue,
            RefBetTypeId = betTypeId,
            SetX = $"{setX}",
        };

        return (wrongResult, actualResult);
    }


    private static string NormalizeHomeValue(string homeValue)
    {
        return homeValue switch
        {
            "T" => "TR", // Tyrant
            "O" => "O", // Overlord
            "NT" => "OM", // Overlord or Middle
            "N" => "None", // None
            _ => string.Empty
        };
    }
}
