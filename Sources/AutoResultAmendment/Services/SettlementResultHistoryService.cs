using System;
using AutoResultAmendment.Criteria;
using AutoResultAmendment.Models;
using AutoResultAmendment.Services;
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
    private readonly PoolService poolService;
    private readonly AppSettings appSettings;

    public SettlementResultHistoryServiceImp(
        SettlementResultHistoryRepository resultHistoryRepository,
        IOptions<AppSettings> appSettings,
        PoolService poolService)
    {
        this.resultHistoryRepository = resultHistoryRepository;
        this.appSettings = appSettings.Value;
        this.poolService = poolService;
    }

    public async Task Handle()
    {
        var result = await poolService.PeekResultHistory();

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
                await resultHistoryRepository.InsResultHistoryTicketDetails(item);
                await resultHistoryRepository.UpdateResultHistory(item, 99);
                return;
            }

            if (!item.HasError && item.IsCalculated && item.IsDoneBalNeg) // success
            {
                await HandleTicketSettleDone(item);
                return;
            }

            if (!item.HasError && (!item.IsCalculated || !item.IsDoneBalNeg)) // fail settle
            {
                await resultHistoryRepository.UpdateResultHistory(item, 3);

                SendMail(item);

                resultHistoryRepository.InsLetsTalkMessage(item);

                return;
            }
        }
        catch (Exception ex)
        {
            LogService.Error("Fanex.AutoResultAmendmentService: ", ex);
        }
        finally
        {
            poolService.Remove(item);
        }
    }

    private async Task HandleTicketSettleDone(ResultHistorySettled item)
    {
        var errorResult = await resultHistoryRepository.CheckResultHistoryError(item);
        var errorSessionIds = errorResult.Select(x => x.SessionId).ToList();
        var errorSessionIdsStr = errorSessionIds.Count > 0 ? string.Join(',', errorSessionIds) : string.Empty;

        await resultHistoryRepository.InsResultHistoryTicketDetails(item, errorSessionIdsStr);

        // Update 2
        await resultHistoryRepository.UpdateResultHistory(item, 2);

        // Update 4
        if (errorResult.Count > 0)
        {
            item.SessionId = errorSessionIdsStr;
            await resultHistoryRepository.UpdateResultHistory(item, 4);
        }
    }

    private void SendMail(ResultHistorySettled item)
    {
        var mailRecord = new MailRecord
        {
            Title = "NAP - Settlement System",
            Subject = "Result Amendment Issue - Process time out",
            MailTo = string.Join(",", appSettings.EmailAddress),
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
                <p>+ Issue: the command has not been completed (success/failed) for more than 5 minutes.</p>
            """;
    }
}