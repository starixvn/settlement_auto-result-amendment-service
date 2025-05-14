namespace ResultAmendment;
public static class ReSettleType
{
    public const short ReCalc = 1;

    public const short RefundToCalc = 3;

    public const short CalcToRefund = 2;

    public const short UnKnow = 0;

    public const short RefundToRefund = 4;
}


public static class ActionType
{
    public const string CalculateAll = "CAL";
    public const string RefundAll = "REFUND";
}


public static class GroupBetTypes
{
    public const int HomeAwayNone = 0;
    public const int HomeAway = 1;
    public const int HomeAwayDraw = 2;
    public const int YesNo = 3;
    public const int CoupleScores = 4;
    public const int Duration = 5;
    public const int MidNotMid = 6;
    public const int SingleScore = 7;
    public const int TopBottomNone = 8;
    public const int TopNotTop = 9;
    public const int BottomNotBottom = 10;
    public const int OverlordTyrant = 11;
    public const int TyrantNotTyrant = 12;
    public const int YesNoNone = 13;

    public static readonly int[] Team = [HomeAwayNone, HomeAwayDraw, HomeAway];
}

