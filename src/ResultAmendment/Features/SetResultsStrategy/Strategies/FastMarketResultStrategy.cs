using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Models;

public class FastMarketResultStrategy : ISetPositionStrategy
{
    public (ResultItem wrongResult, ResultItem actualResult) Generate(IList<ResultChanged> resultChanged, IList<ResourceItem> resourceItem)
    {
        var wrongResult = new ResultItem();
        var actualResult = new ResultItem();

        wrongResult.ResourceId = resultChanged.First().ResourceId;
        actualResult.ResourceId = resourceItem.First().ResourceId;

        wrongResult.Positions = resultChanged.Select(x => new PositionResult
        {
            Position = x.OtherInfo,
            Home = x.HomeValue ?? string.Empty,
            Away = x.AwayValue ?? string.Empty
        }).ToList();

        var actualResultBaseOnResultChanged = resultChanged.Select(o => o.OtherInfo);

        actualResult.Positions = resourceItem.Where(r => actualResultBaseOnResultChanged.Contains(r.OtherInfo)).Select(x => new PositionResult
        {
            Position = x.OtherInfo,
            Home = x.HomeValue ?? string.Empty,
            Away = x.AwayValue ?? string.Empty
        }).ToList();

        return (wrongResult, actualResult);
    }
}
