using System.Text.Json;
using Fanex.Data.Repository;
using Fanex.Settlement.Converter.Services;
using Microsoft.Extensions.Options;
using ResultAmendment.Contracts;
using ResultAmendment.Features.GetMainResource;
using ResultAmendment.Features.ResultHistoryIns;
using ResultAmendment.Features.SetResultsStrategy;
using ResultAmendment.Models;
using Shared;

namespace ResultAmendment.Features.AddSettlementResultHistory;

public class AddSettlementResultHistoryHandler
{
    private readonly IDynamicRepository repository;
    private readonly XmlConvertService xmlConvertService;
    private readonly IGetMainResourceHandler getMainResourceHandler;
    private readonly IOptions<AppSettings> appSettings;

    public AddSettlementResultHistoryHandler(
        IDynamicRepository repository,
        XmlConvertService xmlConvertService,
        IGetMainResourceHandler getMainResourceHandler,
        IOptions<AppSettings> appSettings)
    {
        this.repository = repository;
        this.xmlConvertService = xmlConvertService;
        this.getMainResourceHandler = getMainResourceHandler;
        this.appSettings = appSettings;
    }

    public async Task HandleAsync(ResultAmended resultAmended)
    {
        var mainResourceIds = await getMainResourceHandler.HandlerAsync(resultAmended.SportType);

        var groupedResults = GetTicketsGroupByResettleType(resultAmended.Action, resultAmended.ResultsChanged);
        var resultHistoryItems = groupedResults.Select((group, index) =>
        {
            var context = new CreateResultHistoryItemContext
            {
                ResultsChanged = group.ToList(),
                SelectedResources = resultAmended.SelectedResources,
                ActionType = group.Key,
                SequenceNumber = index + 1,
                MainResourceIds = mainResourceIds
            };

            return CreateResultHistoryItem(context);
        }).ToList();


        if (resultHistoryItems.Count == 0)
        {
            return;
        }

        var criteria = new AddResultHistoryInsCriteria(
            resultAmended.MatchId,
            resultAmended.SportType,
            resultAmended.BroadcastSessionId,
            JsonSerializer.Serialize(resultHistoryItems));

        await ExecSettleResultHistoryInsAllSites(criteria);
    }

    private async Task ExecSettleResultHistoryInsAllSites(AddResultHistoryInsCriteria criteria)
    {
        var servers = appSettings.Value.SiteDbServer.Where(x => x != "DBTXN");

        var tasks = servers.Select(server => ExecSettleResultHistoryInsOnSite(server, criteria)).ToList();

        await Task.WhenAll(tasks);
    }

    private async Task ExecSettleResultHistoryInsOnSite(string server, AddResultHistoryInsCriteria criteria)
    {
        criteria.DbServer = server;
        await repository.ExecuteAsync(criteria);
    }

    private (List<ResultItem> WrongResults, List<ResultItem> ActualResults) GenerateWrongAndActualResults(
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

    private SettlementResultHistoryItem CreateResultHistoryItem(CreateResultHistoryItemContext context)
    {
        var (wrongResults, actualResults) = GenerateWrongAndActualResults(
            context.ResultsChanged,
            context.SelectedResources,
            context.MainResourceIds);

        return new SettlementResultHistoryItem
        {
            SeqNo = context.SequenceNumber,
            WrongResult = xmlConvertService.Serialize(new ResultInfo { ResultItems = wrongResults }),
            ActualResult = xmlConvertService.Serialize(new ResultInfo { ResultItems = actualResults }),
            ExtraInfo = GenerateExtraInfo(context.SelectedResources, context.ResultsChanged),
            ActionType = context.ActionType
        };
    }

    private List<IGrouping<short, ResultChanged>> GetTicketsGroupByResettleType(string action, List<ResultChanged> results)
    {
        results.ForEach(x => x.ReSettleType = GetReSettleActionType(x.Action, action));

        var resultsChangedGroupBy = results
            .Where(x => x.ReSettleType != ReSettleType.UnKnow)
            .GroupBy(x => x.ReSettleType).ToList();

        return resultsChangedGroupBy;
    }

    private static short GetReSettleActionType(string beforeAction, string currentAction)
    {
        if (string.IsNullOrWhiteSpace(beforeAction) || string.IsNullOrWhiteSpace(currentAction))
        {
            return ReSettleType.UnKnow;
        }

        var before = beforeAction.ToUpperInvariant();
        var current = currentAction.ToUpperInvariant();

        return (before, current) switch
        {
            var (b, c) when b.Contains(ActionType.RefundAll) && c.Contains(ActionType.CalculateAll) => ReSettleType.RefundToCalc,
            var (b, c) when b.Contains(ActionType.CalculateAll) && c.Contains(ActionType.RefundAll) => ReSettleType.CalcToRefund,
            var (b, c) when b.Contains(ActionType.CalculateAll) && c.Contains(ActionType.CalculateAll) => ReSettleType.ReCalc,
            _ => ReSettleType.UnKnow
        };
    }

    private string GenerateExtraInfo(List<ResourceItem> selectedResources, List<ResultChanged> resultsChanged)
    {
        var extraInfoItems = new List<ExtraInfoItem>();

        foreach (var resource in selectedResources)
        {
            var resultChanged = resultsChanged.Find(y => y.ResourceId == resource.ResourceId);
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
}
