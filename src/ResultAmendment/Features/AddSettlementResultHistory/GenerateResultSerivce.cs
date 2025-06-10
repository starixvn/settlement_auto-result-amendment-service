using Fanex.Settlement.Converter.Services;
using Fanex.Settlement.MessageBridge.Constants;
using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Features.GetFastMarketInfo;
using ResultAmendment.Features.SetResultsStrategy;
using ResultAmendment.Models;
using Results = (System.Collections.Generic.List<ResultAmendment.Models.ResultItem> WrongResults, System.Collections.Generic.List<ResultAmendment.Models.ResultItem> ActualResults);

namespace ResultAmendment.Features.AddSettlementResultHistory;

public interface IGenerateResultService
{
    (List<ResultItem> WrongResults, List<ResultItem> ActualResults) GenerateWrongAndActualResults(
        List<ResultChanged> resultsChanged,
        List<ResourceItem> selectedResources,
        List<string> mainResourceIds);

    (List<ResultItem> WrongResults, List<ResultItem> ActualResults) GenerateFastMarketResult(string action, List<ResultChanged> resultChanged, List<ResourceItem> selectedResources);

    Results GenerateBetBuilderResult(List<ResultChanged> resultChanged, List<ResourceItem> selectedResources);

    string GenerateExtraInfo(List<ResourceItem> selectedResources, List<ResultChanged> resultsChanged, string type);
}

public class GenerateResultService(XmlConvertService xmlConvertService) : IGenerateResultService
{
    public (List<ResultItem> WrongResults, List<ResultItem> ActualResults) GenerateWrongAndActualResults(
        List<ResultChanged> resultsChanged,
        List<ResourceItem> selectedResources,
        List<string> mainResourceIds)
    {
        var wrongResults = new List<ResultItem>();
        var actualResults = new List<ResultItem>();

        foreach (var item in selectedResources)
        {
            var resultChanged = resultsChanged.Find(x => x.ResourceId == item.ResourceId);
            if (resultChanged == null)
            {
                continue;
            }

            var strategy = ResultStrategyFactory.GetStrategy(item.ResourceId, item.GroupBetType, mainResourceIds);
            var (wrongResult, actualResult) = strategy.Generate(resultChanged, item);

            wrongResults.Add(wrongResult);
            actualResults.Add(actualResult);
        }

        return (wrongResults, actualResults);
    }


    public (List<ResultItem> WrongResults, List<ResultItem> ActualResults) GenerateFastMarketResult(string action, List<ResultChanged> resultChanged, List<ResourceItem> selectedResources)
    {
        var wrongResults = new List<ResultItem>();
        var actualResults = new List<ResultItem>();

        foreach (var item in selectedResources)
        {
            var strategy = ResultStrategyFactory.GetPositionResultStrategy(item.ResourceId);
            var resultsChanged = resultChanged.FirstOrDefault(x => x.ResourceId == item.ResourceId && x.OtherInfo == item.OtherInfo);
            if (resultsChanged is not null)
            {
                item.Action = action;
                var (wrongResult, actualResult) = strategy.Generate(resultsChanged, item);

                wrongResults.Add(wrongResult);
                actualResults.Add(actualResult);
            }
        }
        return (wrongResults, actualResults);
    }

    public Results GenerateBetBuilderResult(
        List<ResultChanged> resultChanged,
        List<ResourceItem> selectedResources)
    {
        var wrongResults = new List<ResultItem>();
        var actualResults = new List<ResultItem>();

        foreach (var item in selectedResources)
        {
            var strategy = ResultStrategyFactory.GetBetbuilderResultStrategy();
            var resultsChanged = resultChanged.FirstOrDefault(x => x.ResourceId == item.ResourceId && x.OtherInfo == item.OtherInfo);

            if (resultsChanged is not null)
            {
                var (wrongResult, actualResult) = strategy.Generate(resultsChanged, item);

                wrongResults.Add(wrongResult);
                actualResults.Add(actualResult);
            }
        }
        return (wrongResults, actualResults);
    }

    public string GenerateExtraInfo(List<ResourceItem> selectedResources, List<ResultChanged> resultsChanged, string type)
    {
        var extraInfoItems = GenerateExtraInfoItems(selectedResources, resultsChanged, type);

        var extraInfo = new ExtraInfo { ExtraInfoItems = extraInfoItems, Type = type };

        return xmlConvertService.Serialize(extraInfo);
    }

    private List<ExtraInfoItem> GenerateExtraInfoItems(List<ResourceItem> selectedResources, List<ResultChanged> resultsChanged, string type)
    {
        var extraInfoItems = new List<ExtraInfoItem>();
        var isFastMarket = type == ResultAmendmentType.FastMarket;
        var isBetBuilder = type == "BB";

        foreach (var resource in selectedResources)
        {
            var resultChanged = isFastMarket || isBetBuilder ? resultsChanged.FirstOrDefault(x => x.ResourceId == resource.ResourceId && x.OtherInfo == resource.OtherInfo)
                                        : resultsChanged.FirstOrDefault(x => x.ResourceId == resource.ResourceId);

            if (resultChanged is not null)
            {
                var positionName = isFastMarket ? GetPositionName.Get(resource.ResourceId, resource.OtherInfo) : null;
                var extraInfoItem = new ExtraInfoItem
                {
                    ResourceId = resource.ResourceId,
                    CalcDateTime = resultChanged.LogDate.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    ReCalcDateTime = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    Position = isFastMarket ? resource.OtherInfo : null,
                    PositionName = positionName,
                    MarketId = isBetBuilder ? resource.OtherInfo : null
                };

                extraInfoItems.Add(extraInfoItem);
            }
        }

        return extraInfoItems;
    }

}
