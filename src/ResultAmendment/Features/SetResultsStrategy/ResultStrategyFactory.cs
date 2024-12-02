using ResultAmendment.Features.SetResultsStrategy.Strategies;

namespace ResultAmendment.Features.SetResultsStrategy;

public static class ResultStrategyFactory
{
    public static ISetResultStrategy GetStrategy(string resourceId, int GroupBetType, List<string> mainResource)
    {
        string[] laneResultPatter = ["9113", "9114", "9130", "9135", "9138"];
        string[] yesNoResultPattern = ["9124", "9132", "9133", "9075", "9087"];
        int resourceIdLength = resourceId.Length;

        return resourceId switch
        {
            _ when mainResource.Contains(resourceId) => new MainResultStrategy(),

            "604" or "603" => new TeamResultStrategy(),
            _ when resourceId.StartsWith("606") || resourceId.StartsWith("607") || resourceId.StartsWith("617") => new TeamXXResultStrategy(),
            _ when resourceId.StartsWith("613") => new TeamXXYYResultStrategy(),

            // 1234_XX
            _ when resourceIdLength == 6 && laneResultPatter.Any(resourceId.StartsWith) => new MapXLaneResultStrategy(), // GroupBetType 6, 7, 8, 9, 10
            _ when resourceIdLength == 6 && (resourceId.StartsWith("9141") || resourceId.StartsWith("9142")) => new MapXOverlordTyrantResultStrategy(),// GroupBetType 11,12
            _ when resourceIdLength == 6 && yesNoResultPattern.Any(resourceId.StartsWith) => new MapXYesNoResultStrategy(), // GroupBetType 3 , 13 
            _ when resourceIdLength == 6 && GroupBetTypes.Team.Contains(GroupBetType) => new TeamXXResultStrategy(), //e2 // GroupBetType 0, 1, 2
            _ when resourceIdLength == 6 && GroupBetTypes.Duration == GroupBetType => new TimeResultStrategy(), //e6 // GroupBetType 5
            _ when resourceIdLength == 6 => new MapXResultStrategy(),

            // 1234_XX_YY
            _ when resourceIdLength == 8 && resourceId.StartsWith("9101") => new TeamXYOrdinalResultStrategy(), // GroupBetType 6, 7, 8, 9, 10
            _ when resourceIdLength == 8 && GroupBetTypes.Team.Contains(GroupBetType) => new TeamXXYYResultStrategy(), //e3, e5  GroupBetType 0, 1, 2
            _ when resourceIdLength == 8 && GroupBetTypes.Duration == GroupBetType => new TimeKillsResultStrategy(), //e7 // GroupBetType 5
            _ when resourceIdLength == 8 => new MapXYResultStrategy(), //e4

            _ => new MainResultStrategy()
        };
    }
}