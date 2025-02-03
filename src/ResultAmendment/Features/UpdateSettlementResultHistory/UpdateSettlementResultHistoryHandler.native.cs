using Fanex.Data.Repository;
using Mapster;
using Microsoft.Extensions.Options;
using ResultAmendment.Contracts;
using Shared;

namespace ResultAmendment.Features.UpdateSettlementResultHistory;

public class UpdateSettlementResultHistoryHandler
{
    private readonly IDynamicRepository repository;
    private readonly IOptions<AppSettings> appSettings;

    public UpdateSettlementResultHistoryHandler(IDynamicRepository repository, IOptions<AppSettings> appSettings)
    {
        this.repository = repository;
        this.appSettings = appSettings;
    }

    public async Task HandleAsync(ResultAmendSettled resultAmendSettled)
    {
        var criteria = resultAmendSettled.Adapt<UpdateSettlementResultHistoryCriteria>();
        criteria.StatusId = resultAmendSettled.IsOrg ? (short)1 : (short)0;

        await ExecUpdateSettlementResultHistoryAllSite(criteria);
    }

    private async Task ExecUpdateSettlementResultHistoryAllSite(UpdateSettlementResultHistoryCriteria criteria)
    {
        var servers = appSettings.Value.SiteDbServer.Where(x => x != "DBTXN");

        var tasks = servers.Select(server => ExecUpdateSettlementResultHistoryOnSite(server, criteria)).ToList();

        await Task.WhenAll(tasks);
    }

    private async Task ExecUpdateSettlementResultHistoryOnSite(string server, UpdateSettlementResultHistoryCriteria criteria)
    {
        criteria.DbServer = server;
        await repository.ExecuteAsync(criteria);
    }
}
