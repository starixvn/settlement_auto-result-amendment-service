using Fanex.Data.Repository;
using Fanex.Settlement.Converter.Services;
using Fanex.Settlement.MessageBridge.Constants;
using Fanex.Settlement.MessageBridge.ResultAmendment;
using Microsoft.Extensions.Options;
using ResultAmendment.Features.GetMainResource;
using ResultAmendment.Models;
using Shared;
using System.Text.Json;

namespace ResultAmendment.Features.AddSettlementResultHistory;

public class AddSettlementResultHistoryHandler(
    IDynamicRepository repository,
    XmlConvertService xmlConvertService,
    IGetMainResourceHandler getMainResourceHandler,
    IGenerateResultService generateResultService,
    IOptions<AppSettings> appSettings)
{
    public async Task HandleAsync(ResultAmended resultAmended)
    {
        var mainResourceIds = await getMainResourceHandler.HandlerAsync(resultAmended.SportType);
        var groupedResults = GetTicketsGroupByResettleType(resultAmended.Action, resultAmended.Type, resultAmended.ResultsChanged);

        var resultHistoryItems = groupedResults.Select((group, index) =>
        {
            var context = new CreateResultHistoryItemContext
            {
                ResultsChanged = group.ToList(),
                SelectedResources = resultAmended.SelectedResources,
                ActionType = group.Key,
                Action = resultAmended.Action,
                SequenceNumber = index + 1,
                MainResourceIds = mainResourceIds,
                Type = resultAmended.Type,
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

    private SettlementResultHistoryItem CreateResultHistoryItem(CreateResultHistoryItemContext context)
    {
        var (wrongResults, actualResults) = GetResults(context);

        return new SettlementResultHistoryItem
        {
            SeqNo = context.SequenceNumber,
            WrongResult = SerializeResultInfo(wrongResults, context.Type),
            ActualResult = SerializeResultInfo(actualResults, context.Type),
            ExtraInfo = generateResultService.GenerateExtraInfo(context.SelectedResources, context.ResultsChanged, context.Type),
            ActionType = context.ActionType
        };
    }

    private (List<ResultItem> WrongResults, List<ResultItem> ActualResults) GetResults(CreateResultHistoryItemContext context)
    {
        if (context.Type == ResultAmendmentType.FastMarket)
        {
            return generateResultService.GenerateFastMarketResult(context.Action, context.ResultsChanged, context.SelectedResources);
        }

        return context.Type == ResultAmendmentType.BetBuilder
            ? generateResultService.GenerateBetBuilderResult(context.ResultsChanged, context.SelectedResources)
            : generateResultService.GenerateWrongAndActualResults(context.ResultsChanged, context.SelectedResources, context.MainResourceIds);
    }

    private string SerializeResultInfo(List<ResultItem> items, string type) =>
        xmlConvertService.Serialize(new ResultInfo { ResultItems = items, Type = type });

    private static List<IGrouping<short, ResultChanged>> GetTicketsGroupByResettleType(string action, string type, List<ResultChanged> results)
    {
        if (results.Count == 0)
        {
            return [];
        }

        var isActionMixed = results.Exists(r => r.Action != results[0].Action);

        results.ForEach(x => x.ReSettleType = GetReSettleActionType(x.Action, action, type, isActionMixed));

        var resultsChangedGroupBy = results
            .Where(x => x.ReSettleType != ReSettleType.UnKnow)
            .GroupBy(x => x.ReSettleType).ToList();

        return resultsChangedGroupBy;
    }

    private static short GetReSettleActionType(string beforeAction, string currentAction, string type, bool actionMixed = false)
    {
        if (string.IsNullOrWhiteSpace(beforeAction) || string.IsNullOrWhiteSpace(currentAction))
        {
            return ReSettleType.UnKnow;
        }

        return IsSpecialCase(type, actionMixed)
            ? ReSettleType.FastMarketType
            : GetReSettleTypeFromActions(beforeAction, currentAction);
    }

    private static bool IsSpecialCase(string type, bool actionMixed) =>
        (type == ResultAmendmentType.FastMarket && actionMixed) || type == ResultAmendmentType.BetBuilder;

    private static short GetReSettleTypeFromActions(string beforeAction, string currentAction)
    {
        var before = beforeAction.ToUpperInvariant();
        var current = currentAction.ToUpperInvariant();

        return (before.Contains(ActionType.RefundAll), before.Contains(ActionType.CalculateAll),
                current.Contains(ActionType.RefundAll), current.Contains(ActionType.CalculateAll)) switch
        {
            (true, _, _, true) => ReSettleType.RefundToCalc,
            (_, true, true, _) => ReSettleType.CalcToRefund,
            (_, true, _, true) => ReSettleType.ReCalc,
            (true, _, true, _) => ReSettleType.RefundToRefund,
            _ => ReSettleType.UnKnow
        };
    }
}