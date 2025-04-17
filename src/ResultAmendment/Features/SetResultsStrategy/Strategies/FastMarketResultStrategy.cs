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
        wrongResult.RefBetTypeId = resultChanged.First().ResourceId;
        actualResult.RefBetTypeId = resourceItem.First().ResourceId;

        wrongResult.Positions = resultChanged.Select(x => new PositionResult
        {
            Position = x.OtherInfo,
            Home = x.HomeValue,
            Away = x.AwayValue
        }).ToList();

        actualResult.Positions = resourceItem.Select(x => new PositionResult
        {
            Position = x.OtherInfo,
            Home = x.HomeValue,
            Away = x.AwayValue
        }).ToList();

        return (wrongResult, actualResult);
    }
}
