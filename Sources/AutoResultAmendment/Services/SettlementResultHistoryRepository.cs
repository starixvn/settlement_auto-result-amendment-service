using Fanex.Caching;

namespace AutoResultAmendment.Services;

using Constants;
using Criteria;
using Models;

public interface SettlementResultHistoryRepository
{
    Task<List<FailureResultHistory>> CheckResultHistoryError(ResultHistorySettled item);

    Task<List<ResultHistorySettled>> GetResultHistoryByEnv();

    Task InsResultHistoryTicketDetails(ResultHistorySettled item, string errorSessionIds);

    Task UpdateResultHistory(ResultHistorySettled item, short statusId);

    void InsLetsTalkMessage(ResultHistorySettled item);
}

public class SettlementResultHistoryRepositoryImp(
    IOptions<AppSettings> appSettings,
    IDynamicRepository dynamicRepository,
    ICache cache)
    : SettlementResultHistoryRepository
{
    public async Task<List<ResultHistorySettled>> GetResultHistoryByEnv()
    {
        var (_, leagueId) = await cache.TryGetAsync<int>(CacheKeys.LeagueIdForTest);
        var result = await GetResultHistory();
        var orderedResult = result.OrderBy(x => x.CreatedDate).ToList();

        return appSettings.Value.EnvironmentName switch
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

    public async Task InsResultHistoryTicketDetails(ResultHistorySettled item, string errorSessionIds)
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
        if (appSettings.Value.EnvironmentName != "PRO") { return; }

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

        foreach (var server in appSettings.Value.SiteDbServer.Where(x => !string.Equals(x, DataBaseName.Dbtxn)))
        {
            criteria.DbServer = server;

            var result = (await dynamicRepository.FetchAsync<ResultHistorySettled>(criteria)).ToList();

            result.ForEach(x => x.DbServer = server);

            results.AddRange(result);
        }

        return results;
    }
}