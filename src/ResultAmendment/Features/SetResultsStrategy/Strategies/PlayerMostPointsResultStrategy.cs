using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Models;
using System.Text.Json;

namespace ResultAmendment.Features.SetResultsStrategy.Strategies;

public class PlayerMostPointsResultStrategy : ISetResultStrategy // 680
{
    public (ResultItem wrongResult, ResultItem actualResult) Generate(ResultChanged resultChanged, ResourceItem resourceItem)
    {
        var oddInfos = resourceItem.OddsInfo
            .Select(x => JsonSerializer.Deserialize<OddsInfoModel>(x))
            .ToList();

        var resourceId = resultChanged.ResourceId;
        var betTypeId = resultChanged.ResourceId[..^2];

        var wrongResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = resultChanged.HomeValue,
            RefBetTypeId = betTypeId,
            OtherInfo = GetOtherInfo(oddInfos),
            TeamIds = GetTeamIds(resultChanged.HomeValue, oddInfos)
        };

        var actualResult = new ResultItem
        {
            ResourceId = resourceId,
            Home = resourceItem.HomeValue,
            RefBetTypeId = betTypeId,
            TeamIds = GetTeamIds(resourceItem.HomeValue, oddInfos),
            OtherInfo = GetOtherInfo(oddInfos),
        };

        return (wrongResult, actualResult);
    }

    private static string GetTeamIds(string homeValue, List<OddsInfoModel> oddsInfos)
    {
        var isHomeNumberic = int.TryParse(homeValue, out var home);
        if (!isHomeNumberic || oddsInfos.Count < 2)
        {
            return string.Empty;
        }

        var odds = oddsInfos[home - 1];

        return $"{odds.PlayerId}";
    }

    private static string GetOtherInfo(List<OddsInfoModel> oddsInfos)
        => string.Join(",", oddsInfos.Select(o => o.PlayerId));
}
