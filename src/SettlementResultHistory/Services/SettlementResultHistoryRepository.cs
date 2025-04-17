using Fanex.Caching;
using Fanex.Settlement.MessageBridge.Constants;
using SettlementResultHistory.Constants;
using SettlementResultHistory.Criteria;
using SettlementResultHistory.Models;
using Shared;

namespace SettlementResultHistory.Services;

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
        var leagueIds = await GetLeagueIdsTest();
        var result = await GetResultHistory();
        var orderedResult = result.OrderBy(x => x.CreatedDate).ToList();

        return appSettings.Value.EnvironmentName switch
        {
            "PRO" => leagueIds.Count > 0 ? orderedResult.Where(x => !leagueIds.Contains(x.LeagueId ?? 0)).ToList() : orderedResult.ToList(),
            "UAT" => orderedResult.Where(x => leagueIds.Contains(x.LeagueId ?? 0)).ToList(),
            _ => orderedResult.ToList()
        };
    }

    private async Task<List<int>> GetLeagueIdsTest()
    {
        var result = new List<int>();
        var (_, leagueId) = await cache.TryGetAsync<int>(CacheKeys.LeagueIdForTest);
        var (_, leagueIdsStr) = await cache.TryGetAsync<string>(CacheKeys.LeagueIdsTest);

        var item = (leagueIdsStr ?? string.Empty).Split(',').Select(x => int.TryParse(x, out var value) ? value : 0).ToList();

        result.Add(leagueId);
        result.AddRange(item);

        return result;
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
            DbServer = item.DbServer,
            Type = CheckXmlTypeAttribute(item.ActualResult) ?? ResultAmendmentType.Normal
        };
        await dynamicRepository.ExecuteAsync(criteria);
    }

    private string CheckXmlTypeAttribute(string xmlString)
    {
        if (string.IsNullOrEmpty(xmlString))
        {
            return ResultAmendmentType.Normal;
        }

        try
        {
            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(xmlString);

            if (xmlDoc.DocumentElement != null && xmlDoc.DocumentElement.Attributes["type"] != null)
            {
                return xmlDoc.DocumentElement.Attributes["type"].Value;
            }
        }
        catch (System.Xml.XmlException)
        {
            return ResultAmendmentType.Normal;
        }

        return ResultAmendmentType.Normal;
    }

    public void InsLetsTalkMessage(ResultHistorySettled item)
    {
        if (appSettings.Value.EnvironmentName != "PRO")
        {
            return;
        }

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