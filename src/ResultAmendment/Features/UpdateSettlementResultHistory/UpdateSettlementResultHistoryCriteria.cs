using Fanex.Data;
using Fanex.Data.Repository;

namespace ResultAmendment.Features.UpdateSettlementResultHistory;

public class UpdateSettlementResultHistoryCriteria : NonQueryCommand
{
    public int MatchId { get; set; }
    public short? StatusId { get; set; } = -1;
    public string SessionIds { get; set; }

    [SpParam(Ignored = true)]
    public string DbServer { get; set; }

    public override string GetSettingKey() => $"NAP_SettlementResultHistory_Update_{DbServer}";

    public override bool IsValid() => MatchId > 0;
}