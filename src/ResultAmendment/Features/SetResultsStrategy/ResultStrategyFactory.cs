using ResultAmendment.Features.SetResultsStrategy.Strategies;

namespace ResultAmendment.Features.SetResultsStrategy;

public static class ResultStrategyFactory
{
    public static ISetResultStrategy GetStrategy(string resourceId, int groupBetType, List<string> mainResource)
    {
        if (mainResource.Contains(resourceId))
        {
            return new MainResultStrategy();
        }

        if (IsTeamResult(resourceId))
        {
            return new TeamResultStrategy();
        }

        if (IsTeamXXResult(resourceId))
        {
            return new TeamXXResultStrategy();
        }

        return IsTeamXXYYResult(resourceId) ? new TeamXXYYResultStrategy() : GetStrategyByLength(resourceId, groupBetType);
    }

    private static ISetResultStrategy GetStrategyByLength(string resourceId, int groupBetType)
    {
        return resourceId.Length switch
        {
            6 => GetLengthSixStrategy(resourceId, groupBetType),
            8 => GetLengthEightStrategy(resourceId, groupBetType),
            _ => new MainResultStrategy()
        };
    }

    private static bool IsTeamXXYYResult(string resourceId)
    {
        return resourceId.StartsWith("613") || resourceId.StartsWith("712") || resourceId.StartsWith("713");
    }

    private static bool IsTeamXXResult(string resourceId)
    {
        return resourceId.StartsWith("606") || resourceId.StartsWith("607") || resourceId.StartsWith("617");
    }

    private static bool IsTeamResult(string resourceId)
    {
        return resourceId is "604" or "605" or "3901";
    }

    private static ISetResultStrategy GetLengthSixStrategy(string resourceId, int groupBetType)
    {
        var laneResultPattern = new List<string> { "9113", "9114", "9130", "9135", "9138" };

        var yesNoResultPattern = new List<string> { "9124", "9132", "9133", "9075", "9087" };

        if (laneResultPattern.Exists(resourceId.StartsWith))
        {
            // GroupBetType 6, 7, 8, 9, 10
            return new MapXLaneResultStrategy();
        }

        if (resourceId.StartsWith("9141") || resourceId.StartsWith("9142"))
        {
            // GroupBetType 11,12
            return new MapXOverlordTyrantResultStrategy();
        }

        if (yesNoResultPattern.Exists(resourceId.StartsWith))
        {
            // GroupBetType 3 , 13
            return new MapXYesNoResultStrategy();
        }

        if (GroupBetTypes.Team.Contains(groupBetType))
        {
            //e2 // GroupBetType 0, 1, 2
            return new TeamXXResultStrategy();
        }

        if (GroupBetTypes.Duration == groupBetType)
        {
            //e6 // GroupBetType 5
            return new TimeResultStrategy();
        }

        return new MapXResultStrategy();
    }

    private static ISetResultStrategy GetLengthEightStrategy(string resourceId, int groupBetType)
    {
        if (resourceId.StartsWith("9101"))
        {
            // GroupBetType 6, 7, 8, 9, 10
            return new TeamXYOrdinalResultStrategy();
        }

        if (resourceId.StartsWith("3903"))
        {
            // For Set X Race To Y Points
            return new SetRacePointStrategy();
        }

        if (resourceId.StartsWith("3915"))
        {
            return new SetPointPlayedStrategy();
        }

        if (GroupBetTypes.Team.Contains(groupBetType))
        {
            //e3, e5  GroupBetType 0, 1, 2
            return new TeamXXYYResultStrategy();
        }

        if (GroupBetTypes.Duration == groupBetType)
        {
            //e7 // GroupBetType 5
            return new TimeKillsResultStrategy();
        }

        return new MapXYResultStrategy();
    }
}