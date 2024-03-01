namespace AutoResultAmendment.Criteria;

public class LetstalkMessageInsCriteria : NonQueryCommand
{
    public string Message { get; set; } = string.Empty;

    public override string GetSettingKey() => "DBA_Send_Letstalk";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(Message);
}