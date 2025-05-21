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
                Type = resultAmended.Type
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
        var (wrongResults, actualResults) = context.Type == ResultAmendmentType.FastMarket ?
        generateResultService.GenerateFastMarketResult(context.Action, context.ResultsChanged, context.SelectedResources) : generateResultService.GenerateWrongAndActualResults(
            context.ResultsChanged,
            context.SelectedResources,
            context.MainResourceIds);

        return new SettlementResultHistoryItem
        {
            SeqNo = context.SequenceNumber,
            WrongResult = xmlConvertService.Serialize(new ResultInfo { ResultItems = wrongResults, Type = context.Type }),
            ActualResult = xmlConvertService.Serialize(new ResultInfo { ResultItems = actualResults, Type = context.Type }),
            ExtraInfo = generateResultService.GenerateExtraInfo(context.SelectedResources, context.ResultsChanged, context.Type),
            ActionType = context.ActionType
        };
    }

    private static List<IGrouping<short, ResultChanged>> GetTicketsGroupByResettleType(string action, string type, List<ResultChanged> results)
    {
        if (results.Count == 0)
        {
            return [];
        }

        var isActionMixed = results.Any(r => r.Action != results.First().Action);
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

        if (type == ResultAmendmentType.FastMarket && actionMixed)
        {
            return ReSettleType.FastMarketType;
        }

        var before = beforeAction.ToUpperInvariant();
        var current = currentAction.ToUpperInvariant();

        return (before, current) switch
        {
            var (b, c) when b.Contains(ActionType.RefundAll) && c.Contains(ActionType.CalculateAll) => ReSettleType.RefundToCalc,
            var (b, c) when b.Contains(ActionType.CalculateAll) && c.Contains(ActionType.RefundAll) => ReSettleType.CalcToRefund,
            var (b, c) when b.Contains(ActionType.CalculateAll) && c.Contains(ActionType.CalculateAll) => ReSettleType.ReCalc,
            var (b, c) when b.Contains(ActionType.RefundAll) && c.Contains(ActionType.RefundAll) => ReSettleType.RefundToRefund,
            _ => ReSettleType.UnKnow
        };
    }
}