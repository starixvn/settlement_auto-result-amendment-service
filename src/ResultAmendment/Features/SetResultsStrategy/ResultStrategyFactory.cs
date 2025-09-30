using ResultAmendment.Features.SetResultsStrategy.Strategies;

namespace ResultAmendment.Features.SetResultsStrategy;

public static class ResultStrategyFactory
{
    private static readonly string[] LanePrefixes = ["9113", "9114", "9130", "9135", "9138"];
    private static readonly string[] YesNoPrefixes = ["9124", "9132", "9133", "9075", "9087"];

    private static readonly Dictionary<int, List<(string[] prefixes, Func<ISetResultStrategy> factory)>> SportSpecificRules = new()
    {
        {
            2,
            new List<(string[] prefixes, Func<ISetResultStrategy>)>
            {
                (["680", "681", "682", "683", "684"], () => new PlayerMostPointsResultStrategy()),
                (["677", "679" ], () => new PlayerDoubleResultStrategy()),
                ([ "653", "656", "659", "662", "665", "667", "669", "671", "673", "674"], () => new PlayerPointResultStrategy()),
                (["675", "676","678"], () => new PlayerPerformanceResultStrategy()),

            }
        }
    };

    public static ISetResultStrategy GetPositionResultStrategy(string resourceId)
    {
        return new FastMarketResultStrategy();
    }

    public static ISetResultStrategy GetStrategy(string resourceId, int groupBetType, List<string> mainResource, int sportType)
    {
        if (mainResource.Contains(resourceId))
        {
            return new MainResultStrategy();
        }

        // Refactored sport-specific lookup
        if (TryGetSportSpecificStrategy(sportType, resourceId, out var sportStrategy))
        {
            return sportStrategy;
        }

        return resourceId switch
        {
            "604" or "605" or "3901" => new TeamResultStrategy(),
            _ when StartsWith(resourceId, "606", "607", "617") => new TeamXXResultStrategy(),
            _ when StartsWith(resourceId, "613", "712", "713") => new TeamXXYYResultStrategy(),
            _ => GetByLength(resourceId, groupBetType)
        };
    }

    private static ISetResultStrategy GetByLength(string resourceId, int groupBetType)
    {
        return resourceId.Length switch
        {
            6 => GetLengthSix(resourceId, groupBetType),
            8 => GetLengthEight(resourceId, groupBetType),
            _ => new MainResultStrategy()
        };
    }

    private static ISetResultStrategy GetLengthSix(string resourceId, int groupBetType) =>
        resourceId switch
        {
            _ when StartsWithAny(resourceId, LanePrefixes) => new MapXLaneResultStrategy(),          // GroupBetType 6..10
            _ when StartsWith(resourceId, "9141", "9142") => new MapXOverlordTyrantResultStrategy(),// GroupBetType 11,12
            _ when StartsWithAny(resourceId, YesNoPrefixes) => new MapXYesNoResultStrategy(),         // GroupBetType 3,13
            _ when GroupBetTypes.Team.Contains(groupBetType) => new TeamXXResultStrategy(),            // GroupBetType 0,1,2
            _ when GroupBetTypes.Duration == groupBetType => new TimeResultStrategy(),              // GroupBetType 5
            _ => new MapXResultStrategy()
        };

    private static ISetResultStrategy GetLengthEight(string resourceId, int groupBetType) =>
        resourceId switch
        {
            _ when resourceId.StartsWith("9101") => new TeamXYOrdinalResultStrategy(),     // GroupBetType 6..10
            _ when resourceId.StartsWith("3903") => new SetRacePointStrategy(),
            _ when resourceId.StartsWith("3915") => new SetPointPlayedStrategy(),
            _ when GroupBetTypes.Team.Contains(groupBetType) => new TeamXXYYResultStrategy(),          // GroupBetType 0,1,2
            _ when GroupBetTypes.Duration == groupBetType => new TimeKillsResultStrategy(),         // GroupBetType 5
            _ => new MapXYResultStrategy()
        };

    private static bool StartsWithAny(string value, string[] prefixes) =>
        prefixes.Any(value.StartsWith);

    private static bool StartsWith(string value, params string[] prefixes)
        => prefixes.Any(value.StartsWith);


    private static bool TryGetSportSpecificStrategy(int sportType, string resourceId, out ISetResultStrategy strategy)
    {
        strategy = default!;
        if (!SportSpecificRules.TryGetValue(sportType, out var rules))
        {
            return false;
        }

        var (prefixes, factory) = rules
            .FirstOrDefault(rule => rule.prefixes.Any(prefix => resourceId.StartsWith(prefix, StringComparison.Ordinal)));

        if (prefixes != null)
        {
            strategy = factory();
            return true;
        }

        return false;
    }

}