using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Models;
using System.Text.Json;

namespace ResultAmendment.Features.SetResultsStrategy.Strategies;

public class PlayerPointResultStrategy : ISetResultStrategy
{
    public (ResultItem wrongResult, ResultItem actualResult) Generate(ResultChanged resultChanged, ResourceItem resourceItem)
    {
        var oddInfoModel = JsonSerializer.Deserialize<OddsInfoModel>(resourceItem.OtherInfo);
        var resourceId = resultChanged.ResourceId;
        var betTypeId = resultChanged.ResourceId[..^2];

        var wrongResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = resultChanged.HomeValue,
            Away = resultChanged.AwayValue,
            RefBetTypeId = betTypeId,
            OtherInfo = $"{oddInfoModel.PlayerId}"
        };

        var actualResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = resourceItem.HomeValue,
            Away = resourceItem.AwayValue,
            RefBetTypeId = betTypeId,
            OtherInfo = $"{oddInfoModel.PlayerId}"
        };

        return (wrongResult, actualResult);
    }
}
