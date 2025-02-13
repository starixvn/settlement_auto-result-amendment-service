namespace ResultAmendment.Features.AddSettlementResultHistory;

public class SettlementResultHistoryItem
{
    public int SeqNo { get; set; }
    public string WrongResult { get; set; }
    public string ActualResult { get; set; }
    public int ActionType { get; set; }
    public string ExtraInfo { get; set; }
}