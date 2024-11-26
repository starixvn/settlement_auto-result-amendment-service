using Fanex.Logging;
using Fanex.MailDeliver.MailClient.Model;

namespace AutoResultAmendment.Services;

using Constants;
using Models;

public interface SettlementResultHistoryService
{
    Task Handle();
}

public class SettlementResultHistoryServiceImp(
    SettlementResultHistoryRepository resultHistoryRepository,
    IOptions<AppSettings> appSettings)
    : SettlementResultHistoryService
{
    public async Task Handle()
    {
        var result = await resultHistoryRepository.GetResultHistoryByEnv();
        foreach (var item in result)
        {
            await Process(item);
        }
    }

    private async Task Process(ResultHistorySettled item)
    {
        try
        {
            if (item.HasError)
            {
                await resultHistoryRepository.InsResultHistoryTicketDetails(item, string.Empty);
                await resultHistoryRepository.UpdateResultHistory(item, ResultHistoryStatus.Error);
                return;
            }

            if (item is { HasError: false, IsCalculated: true, IsDoneBalNeg: true })
            {
                await HandleTicketSettleDone(item);
                return;
            }

            if (!item.HasError && (!item.IsCalculated || !item.IsDoneBalNeg))
            {
                await resultHistoryRepository.InsResultHistoryTicketDetails(item, string.Empty);
                await resultHistoryRepository.UpdateResultHistory(item, ResultHistoryStatus.Failure);

                SendMail(item);

                resultHistoryRepository.InsLetsTalkMessage(item);
            }
        }
        catch (Exception ex)
        {
            await LogService.ErrorAsync("Fanex.AutoResultAmendmentService: ", ex);
        }
    }

    private async Task HandleTicketSettleDone(ResultHistorySettled item)
    {
        var errorResult = await resultHistoryRepository.CheckResultHistoryError(item);
        var errorSessionIds = errorResult.Select(x => x.SessionId).ToList();
        var errorSessionIdsStr = errorSessionIds.Count > 0 ? string.Join(',', errorSessionIds) : string.Empty;

        await resultHistoryRepository.InsResultHistoryTicketDetails(item, errorSessionIdsStr);

        await resultHistoryRepository.UpdateResultHistory(item, ResultHistoryStatus.Success);

        if (errorResult.Count > 0)
        {
            item.SessionId = errorSessionIdsStr;
            await resultHistoryRepository.UpdateResultHistory(item, ResultHistoryStatus.ErrorHandled);
        }
    }

    private void SendMail(ResultHistorySettled item)
    {
        var mailRecord = new MailRecord
        {
            Title = "NAP - Settlement System",
            Subject = "Result Amendment Issue - Process time out",
            MailTo = string.Join(",", appSettings.Value.EmailAddress),
            Body = MailBody(item)
        };

        using var mailClient = new Fanex.MailDeliver.MailClient.API.MailDeliver();

        mailClient.SendMail(mailRecord);
    }

    private static string MailBody(ResultHistorySettled item)
    {
        return $"""
            <p>- Match ID: {item.MatchId}</p>
            <p>- Amendment Information</p>
            <div style="padding-left:20px">
                <p>+ SiteDB: {item.DbServer}</p>
                <p>+ Session ID: {item.SessionId}</p>
                <p>+ Action: {item.ActionType}</p>
                <p>+ Created Time: {item.CreatedDate}</p>
                <p>+ Current Status: {item.StatusId}</p>
                <p>+ Issue: Process time out. The command has not been completed (success/failed) for more than 5 minutes.</p>
            """;
    }
}