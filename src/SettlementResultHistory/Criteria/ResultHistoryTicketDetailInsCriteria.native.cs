namespace SettlementResultHistory.Criteria;

public class ResultHistoryTicketDetailInsCriteria : NonQueryCommand
{
    public int MatchId { get; set; }

    public int SportType { get; set; }

    public string SessionId { get; set; }

    public string ErrorSessionIds { get; set; }

    [SpParam(Ignored = true)]
    public string DbServer { get; set; } = string.Empty;

    public override string GetSettingKey() => $"NAP_SettlementResultHistory_TicketDetails_Ins_{DbServer}";

    public override bool IsValid() => true;
}