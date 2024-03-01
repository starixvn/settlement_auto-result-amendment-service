namespace AutoResultAmendment.Models;

public class ResultHistorySettled
{
    public int MatchId { get; set; }
    public int? LeagueId { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public int SportType { get; set; }
    public short ActionType { get; set; }
    public string ActualResult { get; set; } = string.Empty;
    public string ExtraInfo { get; set; } = string.Empty;
    public short StatusId { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsCalculated { get; set; }
    public bool IsDoneBalNeg { get; set; }
    public bool HasError { get; set; }
    public string DbServer { get; set; } = string.Empty;

    public override bool Equals(object? obj)
    {
        return obj is ResultHistorySettled item
               && item.SessionId == SessionId
               && item.DbServer == DbServer;
    }

    public override int GetHashCode() => SessionId.GetHashCode() ^ DbServer.GetHashCode();
}