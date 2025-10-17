using Fanex.Settlement.MessageBridge.Constants;

namespace SettlementResultHistory.Criteria;

public class ResultHistoryTicketDetailInsCriteria : NonQueryCommand
{
    public int MatchId { get; set; }

    public int SportType { get; set; }

    public string SessionId { get; set; }

    public string ErrorSessionIds { get; set; }

    [SpParam(Ignored = true)]
    public string DbServer { get; set; } = string.Empty;

    [SpParam(Ignored = true)]
    public string Type { get; set; } = ResultAmendmentType.Normal;

    public override string GetSettingKey() => Type switch
    {
        "FM" => $"NAP_SettlementResultHistory_TicketDetails_FM_Ins_{DbServer}",
        "BB" => $"NAP_SettlementResultHistory_TicketDetails_BB_Ins_{DbServer}",
        _ => $"NAP_SettlementResultHistory_TicketDetailIns_{DbServer}"
    };

    public override bool IsValid() => true;
}