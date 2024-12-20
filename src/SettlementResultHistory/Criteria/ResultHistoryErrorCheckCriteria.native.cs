namespace SettlementResultHistory.Criteria;

public class ResultHistoryErrorCheckCriteria : CriteriaBase
{
    public int MatchId { get; set; }

    public string SessionId { get; set; }

    public DateTime CreatedDate { get; set; }

    public string ActualResult { get; set; }

    [SpParam(Ignored = true)]
    public string DbServer { get; set; } = string.Empty;

    public override string GetSettingKey() => $"NAP_SettlementResultHistory_CheckErrorStatus_{DbServer}";

    public override bool IsValid() => MatchId > 0;
}