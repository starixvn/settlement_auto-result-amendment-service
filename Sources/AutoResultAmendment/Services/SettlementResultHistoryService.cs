using AutoResultAmendment.Criteria;
using AutoResultAmendment.Models;
using Fanex.Caching;
using Fanex.Data.Repository;
using Fanex.Logging;
using Fanex.MailDeliver.MailClient.Model;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;

public interface SettlementResultHistoryService
{
    Task Handle();
}

public class SettlementResultHistoryServiceImp : SettlementResultHistoryService
{
    private readonly SettlementResultHistoryRepository resultHistoryRepository;
    private readonly AppSettings appSettings;

    public SettlementResultHistoryServiceImp(SettlementResultHistoryRepository resultHistoryRepository, IOptions<AppSettings> appSettings)
    {
        this.resultHistoryRepository = resultHistoryRepository;
        this.appSettings = appSettings.Value;
    }

    public async Task Handle()
    {
        var result = await resultHistoryRepository.GetResultHistoryByEnv();

        foreach (var item in result)
        {
            if (!item.HasError && item.IsCalculated && item.IsDoneBalNeg) // success
            {
                var checkResultCriteria = new ResultHistoryErrorCheckCriteria
                {
                    SessionId = item.SessionId,
                    CreatedDate = item.CreatedDate,
                    ActualResult = item.ActualResult,
                    MatchId = item.MatchId,
                    DbServer = item.DbServer,
                };

                var errorResult = await resultHistoryRepository.CheckResultHistoryError(checkResultCriteria);
                var errorSessionIds = errorResult.Select(x => x.SessionId).ToArray();

                await resultHistoryRepository.InsResultHistoryTicketDetails(new()
                {
                    MatchId = item.MatchId,
                    SessionId = item.SessionId,
                    SportType = item.SportType,
                    ErrorSessionIds = errorSessionIds.Length > 0 ? string.Join(',', errorSessionIds) : string.Empty,
                    DbServer = item.DbServer
                });

                // Update 2
                var successUpdateCriteria = new ResultHistoryUpdateCriteria
                {
                    SessionIds = item.SessionId,
                    StatusId = 2,
                    MatchId = item.MatchId,
                    DbServer = item.DbServer
                };
                await resultHistoryRepository.UpdateResultHistory(successUpdateCriteria);

                // Update 4
                if (errorResult.Count > 0)
                {
                    var sessionIds = errorResult.Select(x => x.SessionId).ToArray();
                    await resultHistoryRepository.UpdateResultHistory(new()
                    {
                        SessionIds = string.Join(",", sessionIds),
                        StatusId = 4,
                        MatchId = item.MatchId,
                        DbServer = item.DbServer
                    });
                }

                return;
            }

            if (!item.HasError && (!item.IsCalculated || !item.IsDoneBalNeg)) // fail settle
            {
                await resultHistoryRepository.UpdateResultHistory(new()
                {
                    SessionIds = item.SessionId,
                    StatusId = 3,
                    MatchId = item.MatchId,
                    DbServer = item.DbServer
                });

                SendMail(item, item.DbServer);
                resultHistoryRepository.InsLetsTalkMessage(item, item.DbServer);
            }

            if (item.HasError)
            {
                await resultHistoryRepository.InsResultHistoryTicketDetails(new()
                {
                    MatchId = item.MatchId,
                    SessionId = item.SessionId,
                    SportType = item.SportType,
                    DbServer = item.DbServer
                });

                await resultHistoryRepository.UpdateResultHistory(new()
                {
                    SessionIds = item.SessionId,
                    StatusId = 99,
                    MatchId = item.MatchId,
                    DbServer = item.DbServer
                });
            }
        }
    }

    private void SendMail(ResultHistorySettled item, string dbServer)
    {
        var mailRecord = new MailRecord
        {
            Title = "NAP - Settlement System",
            Subject = "Result Amendment Issue - Process time out",
            MailTo = string.Join(",", appSettings.EmailAddress),
            Body = MailBody(item, dbServer)
        };

        using var mailClient = new Fanex.MailDeliver.MailClient.API.MailDeliver();

        mailClient.SendMail(mailRecord);
    }

    private static string MailBody(ResultHistorySettled item, string dbServer)
    {
        return $"""
            <p>- Match ID: {item.MatchId}</p>
            <p>- Amendment Information</p>
            <div style="padding-left:20px">
                <p>+ SiteDB: {dbServer}</p>
                <p>+ Session ID: {item.SessionId}</p>
                <p>+ Action: {item.ActionType}</p>
                <p>+ Created Time: {item.CreatedDate}</p>
                <p>+ Current Status: {item.StatusId}</p>
                <p>+ Issue: the command has not been completed (success/failed) for more than 5 minutes.</p>
            """;
    }
}