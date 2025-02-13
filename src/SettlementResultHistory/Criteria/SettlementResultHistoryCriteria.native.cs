namespace SettlementResultHistory.Criteria;

public class SettlementResultHistoryCriteria : NonQueryCommand
{
    [SpParam(Ignored = true)]
    public string DbServer { get; set; } = string.Empty;

    public override bool IsValid() => true;

    public override string GetSettingKey() => $"NAP_SettlementResultHistory_Schedule_{DbServer}";
}