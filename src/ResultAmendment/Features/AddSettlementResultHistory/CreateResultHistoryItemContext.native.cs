using Fanex.Settlement.MessageBridge.ResultAmendment;

namespace ResultAmendment.Features.AddSettlementResultHistory;
public class CreateResultHistoryItemContext
{
    public List<ResultChanged> ResultsChanged { get; set; }
    public List<ResourceItem> SelectedResources { get; set; }
    public short ActionType { get; set; }
    public int SequenceNumber { get; set; }
    public List<string> MainResourceIds { get; set; }
}
