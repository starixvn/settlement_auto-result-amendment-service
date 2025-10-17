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
        var (wrongResults, actualResults) = context.Type switch
        {
            "FM" => generateResultService.GenerateFastMarketResult(context.Action, context.ResultsChanged, context.SelectedResources),
            "BB" => generateResultService.GenerateBetBuilderResult(context.ResultsChanged, context.SelectedResources),
            _ => generateResultService.GenerateWrongAndActualResults(context.ResultsChanged, context.SelectedResources, context.MainResourceIds)
        };


        return new SettlementResultHistoryItem
        {
            SeqNo = context.SequenceNumber,
            WrongResult = SerializeResultInfo(wrongResults, context.Type),
            ActualResult = SerializeResultInfo(actualResults, context.Type),
            ExtraInfo = generateResultService.GenerateExtraInfo(context.SelectedResources, context.ResultsChanged, context.Type),
            ActionType = context.ActionType
        };
    }

    private string SerializeResultInfo(List<ResultItem> items, string type)
        => xmlConvertService.Serialize(new ResultInfo { ResultItems = items, Type = type });

    private static List<IGrouping<short, ResultChanged>> GetTicketsGroupByResettleType(string action, string type, List<ResultChanged> results)
    {
        if (results.Count == 0)
        {
            return [];
        }

        results.ForEach(x => x.ReSettleType = GetReSettleActionType(x.Action, action, type));
        var resultsChangedGroupBy = results
            .Where(x => x.ReSettleType != ReSettleType.UnKnow)
            .GroupBy(x => x.ReSettleType).ToList();

        return resultsChangedGroupBy;
    }

    private static short GetReSettleActionType(string beforeAction, string currentAction, string type)
    {
        if (string.IsNullOrWhiteSpace(beforeAction) || string.IsNullOrWhiteSpace(currentAction))
        {
            return ReSettleType.UnKnow;
        }

        return type == ResultAmendmentType.FastMarket || type == "BB"
            ? ReSettleType.FastMarketType
            : DetermineActionType(beforeAction.ToUpperInvariant(), currentAction.ToUpperInvariant());
    }

    private static short DetermineActionType(string before, string current)
    {
        if (IsRefundToCalculate(before, current))
        {
            return ReSettleType.RefundToCalc;
        }

        if (IsCalculateToRefund(before, current))
        {
            return ReSettleType.CalcToRefund;
        }

        return IsRecalculate(before, current) ? ReSettleType.ReCalc : ReSettleType.UnKnow;
    }

    private static bool IsRefundToCalculate(string before, string current)
    {
        return before.Contains(ActionType.RefundAll) && current.Contains(ActionType.CalculateAll);
    }

    private static bool IsCalculateToRefund(string before, string current)
    {
        return before.Contains(ActionType.CalculateAll) && current.Contains(ActionType.RefundAll);
    }

    private static bool IsRecalculate(string before, string current)
    {
        return before.Contains(ActionType.CalculateAll) && current.Contains(ActionType.CalculateAll);
    }
}