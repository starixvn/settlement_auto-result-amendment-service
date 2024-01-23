using Fanex.Data;
using Fanex.Data.Repository;

namespace AutoResultAmendment.Criteria;

public class SettlementResultHistoryCriteria : NonQueryCommand
{
    [SpParam(Ignored = true)]
    public string DbServer { get; set; } = string.Empty;

    public override bool IsValid() => !string.IsNullOrWhiteSpace(DbServer);

    public override string GetSettingKey() => $"NAP_SettlementResultHistory_Schedule_{DbServer}";
}