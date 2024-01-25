using System.Reflection.Emit;
using AutoResultAmendment.Criteria;
using AutoResultAmendment.Models;
using Fanex.Caching;
using Fanex.Data.Repository;
using Fanex.Logging;
using Fanex.MailDeliver.MailClient.Model;
using Microsoft.Extensions.Options;

public interface SettlementResultHistoryRepository
{
    Task<List<FailureResultHistory>> CheckResultHistoryError(ResultHistorySettled item);

    Task<List<ResultHistorySettled>> GetResultHistoryByEnv();

    Task InsResultHistoryTicketDetails(ResultHistorySettled item, string errorSessionIds = "");

    Task UpdateResultHistory(ResultHistorySettled item, short StatusId);

    void InsLetsTalkMessage(ResultHistorySettled item);
}

public class SettlementResultHistoryRepositoryImp : SettlementResultHistoryRepository
{
    private const string KeyLeagueId = "Settlement-AutoPayoutVirtualCricket-LeagueId";

    private readonly AppSettings appSettings;
    private readonly IDynamicRepository dynamicRepository;
    private readonly ICache cache;

    public SettlementResultHistoryRepositoryImp(
        IOptions<AppSettings> appSettings,
        IDynamicRepository dynamicRepository,
        ICache cache)
    {
        this.appSettings = appSettings.Value;
        this.dynamicRepository = dynamicRepository;
        this.cache = cache;
    }

    public async Task<List<ResultHistorySettled>> GetResultHistoryByEnv()
    {
        var (_, leagueId) = await cache.TryGetAsync<int>(KeyLeagueId);
        var result = await GetResultHistory();
        var orderedResult = result.OrderBy(x => x.CreatedDate).ToList();

        return appSettings.EnvironmentName switch
        {
            "PRO" => leagueId > 0 ? orderedResult.Where(x => x.LeagueId != leagueId).ToList() : orderedResult.ToList(),
            "UAT" => orderedResult.Where(x => x.LeagueId == leagueId).ToList(),
            _ => orderedResult.ToList()
        };
    }

    public async Task UpdateResultHistory(ResultHistorySettled item, short statusId)
    {
        var criteria = new ResultHistoryUpdateCriteria
        {
            SessionIds = item.SessionId,
            StatusId = statusId,
            MatchId = item.MatchId,
            DbServer = item.DbServer
        };
        await dynamicRepository.ExecuteAsync(criteria);
    }

    public async Task InsResultHistoryTicketDetails(ResultHistorySettled item, string errorSessionIds = "")
    {
        var criteria = new ResultHistoryTicketDetailInsCriteria
        {
            MatchId = item.MatchId,
            SessionId = item.SessionId,
            ErrorSessionIds = errorSessionIds,
            SportType = item.SportType,
            DbServer = item.DbServer
        };
        await dynamicRepository.ExecuteAsync(criteria);
    }

    public void InsLetsTalkMessage(ResultHistorySettled item)
    {
        if (appSettings.EnvironmentName != "PRO") { return; }

        var message = $"<p>Hey guys, Result Amendment Notification at {item.DbServer} has not been completed" +
            "</br>Please check data at <b>bodb02.dbo.SettlementResultHistory</b>" +
            $"</br><b>-Match ID:</b> {item.MatchId}" +
            $"</br><b>-Session:</b> {item.SessionId}</p>";

        var criteria = new LetstalkMessageInsCriteria
        {
            Message = message,
        };

        dynamicRepository.Execute(criteria);
    }

    public async Task<List<FailureResultHistory>> CheckResultHistoryError(ResultHistorySettled item)
    {
        var criteria = new ResultHistoryErrorCheckCriteria
        {
            SessionId = item.SessionId,
            CreatedDate = item.CreatedDate,
            ActualResult = item.ActualResult,
            MatchId = item.MatchId,
            DbServer = item.DbServer,
        };

        var result = await dynamicRepository.FetchAsync<FailureResultHistory>(criteria);

        return result.ToList();
    }

    private async Task<List<ResultHistorySettled>> GetResultHistory()
    {
        var criteria = new SettlementResultHistoryCriteria();
        var results = new List<ResultHistorySettled>();

        foreach (var server in appSettings.SiteDbServer.Where(x => x != "DBTXN"))
        {
            criteria.DbServer = server;

            var result = (await dynamicRepository.FetchAsync<ResultHistorySettled>(criteria)) ?? new List<ResultHistorySettled>();

            result.ToList().ForEach(x => x.DbServer = server);

            results.AddRange(result);
        }

        return results;
    }
}