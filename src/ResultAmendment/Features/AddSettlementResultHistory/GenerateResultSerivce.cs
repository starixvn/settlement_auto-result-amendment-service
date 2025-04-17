using Fanex.Settlement.Converter.Services;
using Fanex.Settlement.MessageBridge.Constants;
using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Features.SetResultsStrategy;
using ResultAmendment.Models;

namespace ResultAmendment.Features.AddSettlementResultHistory;

public interface IGenerateResultService
{
    (List<ResultItem> WrongResults, List<ResultItem> ActualResults) GenerateWrongAndActualResults(
        List<ResultChanged> resultsChanged,
        List<ResourceItem> selectedResources,
        List<string> mainResourceIds);

    (List<ResultItem> WrongResults, List<ResultItem> ActualResults) GenerateFastMarketResult(List<ResultChanged> resultChanged, List<ResourceItem> selectedResources);

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


    public (List<ResultItem> WrongResults, List<ResultItem> ActualResults) GenerateFastMarketResult(List<ResultChanged> resultChanged, List<ResourceItem> selectedResources)
    {
        var wrongResults = new List<ResultItem>();
        var actualResults = new List<ResultItem>();

        var selectedResourcesGroupByResourceId = selectedResources.GroupBy(x => x.ResourceId).ToList();

        foreach (var item in selectedResourcesGroupByResourceId)
        {
            var strategy = ResultStrategyFactory.GetPositionResultStrategy(item.Key);
            var resultsChanged = resultChanged.Where(x => x.ResourceId == item.Key).ToList();
            if (resultsChanged.Any())
            {
                var (wrongResult, actualResult) = strategy.Generate(resultsChanged, item.ToList());

                wrongResults.Add(wrongResult);
                actualResults.Add(actualResult);
            }
        }
        return (wrongResults, actualResults);
    }

    public string GenerateExtraInfo(List<ResourceItem> selectedResources, List<ResultChanged> resultsChanged, string type)
    {
        var extraInfoItems = new List<ExtraInfoItem>();

        foreach (var resource in selectedResources)
        {
            var resultChanged = GetResultChanged(resultsChanged, resource, type);
            if (resultChanged == null)
            {
                continue;
            }

            var extraInfoItem = new ExtraInfoItem
            {
                ResourceId = resultChanged.ResourceId,
                CalcDateTime = resultChanged.LogDate.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                ReCalcDateTime = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
            };

            extraInfoItems.Add(extraInfoItem);
        }

        var extraInfo = new ExtraInfo { ExtraInfoItems = extraInfoItems };

        return xmlConvertService.Serialize(extraInfo);
    }

    private ResultChanged GetResultChanged(List<ResultChanged> resultsChanged, ResourceItem resourceItem, string type)
    {
        return resultsChanged.FirstOrDefault(x =>
            type == ResultAmendmentType.FastMarket
                ? x.ResourceId == resourceItem.ResourceId && x.OtherInfo == resourceItem.OtherInfo
                : x.ResourceId == resourceItem.ResourceId);
    }
}
