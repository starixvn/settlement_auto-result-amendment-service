using ResultAmendment.Models;

namespace ResultAmendment.Contracts;

public record class ResultAmended(
    int MatchId,
    string BroadcastSessionId,
    int SportType,
    string Action,
    int? HomeId,
    int? AwayId,
    List<ResourceItem> SelectedResources,
    List<ResultChanged> ResultsChanged);
