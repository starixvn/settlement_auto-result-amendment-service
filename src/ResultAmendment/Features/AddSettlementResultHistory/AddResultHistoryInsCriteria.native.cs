using Fanex.Data;
using Fanex.Data.Repository;

namespace ResultAmendment.Features.AddSettlementResultHistory;

public class AddResultHistoryInsCriteria(int matchId, int sportType, string sessionID, string resourceInfo) : NonQueryCommand
{
    public int MatchId { get; set; } = matchId;

    public int SportType { get; set; } = sportType;

    public string SessionID { get; set; } = sessionID;

    public string ResourceInfo { get; set; } = resourceInfo;

    public short ResultType { get; set; } = 1;

    [SpParam(Ignored = true)]
    public string DbServer { get; set; }

    public override string GetSettingKey() => $"NAP_SettlementResultHistory_Ins_{DbServer}";

    public override bool IsValid() => MatchId > 0 && !string.IsNullOrEmpty(DbServer);
}