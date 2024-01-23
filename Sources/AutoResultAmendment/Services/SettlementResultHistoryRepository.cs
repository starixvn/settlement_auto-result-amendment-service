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
    Task<List<FailureResultHistory>> CheckResultHistoryError(ResultHistoryErrorCheckCriteria criteria);

    Task<List<ResultHistorySettled>> GetResultHistoryByEnv();

    Task InsResultHistoryTicketDetails(ResultHistoryTicketDetailInsCriteria criteria);

    Task UpdateResultHistory(ResultHistoryUpdateCriteria criteria);

    void InsLetsTalkMessage(ResultHistorySettled item, string dbServer);
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

    public async Task UpdateResultHistory(ResultHistoryUpdateCriteria criteria)
    {
        await dynamicRepository.ExecuteAsync(criteria);
    }

    public async Task InsResultHistoryTicketDetails(ResultHistoryTicketDetailInsCriteria criteria)
    {
        await dynamicRepository.ExecuteAsync(criteria);
    }

    public void InsLetsTalkMessage(ResultHistorySettled item, string dbServer)
    {
        if (appSettings.EnvironmentName != "PRO") { return; }

        var message = $"<p>Hey guys, Result Amendment Notification at {dbServer} has not been completed" +
            "</br>Please check data at <b>bodb02.dbo.SettlementResultHistory</b>" +
            $"</br><b>-Match ID:</b> {item.MatchId}" +
            $"</br><b>-Session:</b> {item.SessionId}</p>";

        var criteria = new LetstalkMessageInsCriteria
        {
            Message = message,
        };

        dynamicRepository.Execute(criteria);
    }

    public async Task<List<FailureResultHistory>> CheckResultHistoryError(ResultHistoryErrorCheckCriteria criteria)
    {
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

            var result = await dynamicRepository.FetchAsync<ResultHistorySettled>(criteria);

            result.ToList().ForEach(x => x.DbServer = server);

            results.AddRange(result);
        }

        return results;
    }
}