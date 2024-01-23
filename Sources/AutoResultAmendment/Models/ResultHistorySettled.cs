using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoResultAmendment.Models;

public class ResultHistorySettled
{
    public int MatchId { get; set; }
    public int LeagueId { get; set; }
    public string SessionId { get; set; }
    public int SportType { get; set; }
    public short ActionType { get; set; }
    public string ActualResult { get; set; }
    public short StatusId { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsCalculated { get; set; }
    public bool IsDoneBalNeg { get; set; }
    public bool HasError { get; set; }
    public string DbServer { get; set; }
}