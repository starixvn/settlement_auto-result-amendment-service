namespace ResultAmendment.Features.GetFastMarketInfo;
public static class GetPositionName
{
    private const int NextOneMinute = 221;

    private const int FirstNextOneMinute = 223;

    private const int NextOneMinuteSetPiece = 225;

    private const int CombinationNextOneMinute = 226;

    private const int TimeMachineMinutes = 228;

    private const int TimeMachineHdp = 229;

    private const int TenMinutes = 10;

    private const int FirstHalfInjuryTime = 45;


    public static string Get(string betType, string position)
    {
        if (!int.TryParse(betType, out var betTypeId) || !int.TryParse(position, out var positionId))
        {
            return string.Empty;
        }

        switch (betTypeId)
        {
            case NextOneMinute:
            case FirstNextOneMinute:
            case NextOneMinuteSetPiece:
            case CombinationNextOneMinute:
                return OneMinuteInterval(positionId);

            case TimeMachineMinutes:
            case TimeMachineHdp:
                {
                    var interval = positionId > FirstHalfInjuryTime ? positionId - 1 : positionId;
                    return positionId == FirstHalfInjuryTime ? "First Half Injury Time" : TimeMachineInterval(interval);
                }
            default:
                return FiveMinutesInterval(positionId);
        }
    }

    private static string OneMinuteInterval(int position)
    {
        return position < TenMinutes ? $"0{position}:00-0{position}:59" : $"{position}:00-{position}:59";
    }

    private static string FiveMinutesInterval(int position)
    {
        var startTime = position < TenMinutes ? $"0{position}:00" : $"{position}:00";

        var endPosition = position + 4;
        var endTime = endPosition < TenMinutes ? $"0{endPosition}:59" : $"{endPosition}:59";

        return $"{startTime}-{endTime}";
    }

    private static string TimeMachineInterval(int position)
    {
        const int nineMinute = 9;

        return position switch
        {
            0 => $"0{position}:00-01:00",
            < nineMinute => $"0{position}:01-0{position + 1}:00",
            nineMinute => $"0{position}:01-{position + 1}:00",
            _ => $"{position}:01-{position + 1}:00"
        };
    }
}
