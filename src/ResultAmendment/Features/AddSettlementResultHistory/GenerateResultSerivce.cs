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
        var extraInfoItems = type == ResultAmendmentType.FastMarket ? FastMarketGenerate(selectedResources, resultsChanged) : NormalResultGenerate(selectedResources, resultsChanged);

        var extraInfo = new ExtraInfo { ExtraInfoItems = extraInfoItems, Type = type };

        return xmlConvertService.Serialize(extraInfo);
    }

    private List<ExtraInfoItem> NormalResultGenerate(List<ResourceItem> selectedResources, List<ResultChanged> resultsChanged)
    {
        var extraInfoItems = new List<ExtraInfoItem>();

        foreach (var resource in selectedResources)
        {
            var resultChanged = resultsChanged.FirstOrDefault(x => x.ResourceId == resource.ResourceId);
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

        return extraInfoItems;
    }

    private List<ExtraInfoItem> FastMarketGenerate(List<ResourceItem> selectedResources, List<ResultChanged> resultsChanged)
    {
        var extraInfoItems = new List<ExtraInfoItem>();

        var bettypeGrouped = selectedResources.GroupBy(r => r.ResourceId);

        foreach (var resource in bettypeGrouped)
        {
            var extraInfoItem = new ExtraInfoItem
            {
                ResourceId = resource.Key,
                Positions = resultsChanged.Where(r => r.ResourceId == resource.Key).Select(r => new PositionResult
                {
                    Position = r.OtherInfo,
                    CalcDateTime = r.LogDate.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    ReCalcDateTime = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                }).ToList()
            };

            extraInfoItems.Add(extraInfoItem);
        }

        return extraInfoItems.Where(r => r.Positions.Count != 0).ToList();
    }
}
