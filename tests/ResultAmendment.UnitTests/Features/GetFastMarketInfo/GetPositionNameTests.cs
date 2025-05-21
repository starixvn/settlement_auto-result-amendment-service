using ResultAmendment.Features.GetFastMarketInfo;

public class GetPositionNameTests
{
    [Theory]
    [InlineData("221", "1", "01:00-01:59")] // NextOneMinute, < 10
    [InlineData("221", "10", "10:00-10:59")] // NextOneMinute, = 10
    [InlineData("223", "5", "05:00-05:59")] // FirstNextOneMinute
    [InlineData("225", "9", "09:00-09:59")] // NextOneMinuteSetPiece
    [InlineData("226", "12", "12:00-12:59")] // CombinationNextOneMinute
    public void Get_Returns_OneMinuteInterval_For_OneMinuteTypes(string betType, string position, string expected)
    {
        var result = GetPositionName.Get(betType, position);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("228", "0", "00:00-01:00")] // TimeMachineMinutes, position 0
    [InlineData("228", "1", "01:01-02:00")] // TimeMachineMinutes, position < 9
    [InlineData("228", "9", "09:01-10:00")] // TimeMachineMinutes, position == 9
    [InlineData("228", "10", "10:01-11:00")] // TimeMachineMinutes, position > 9
    [InlineData("229", "5", "05:01-06:00")] // TimeMachineHdp
    [InlineData("228", "45", "First Half Injury Time")] // TimeMachineMinutes, FirstHalfInjuryTime
    [InlineData("229", "46", "45:01-46:00")] // TimeMachineHdp, position > FirstHalfInjuryTime
    public void Get_Returns_TimeMachineInterval_For_TimeMachineTypes(string betType, string position, string expected)
    {
        var result = GetPositionName.Get(betType, position);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("222", "1", "01:00-05:59")] // NextFiveMinutes, < 10
    [InlineData("222", "10", "10:00-14:59")] // NextFiveMinutes, = 10
    [InlineData("999", "3", "03:00-07:59")] // Default case, unknown betType
    [InlineData("222", "8", "08:00-12:59")] // Edge case, < 10
    [InlineData("222", "12", "12:00-16:59")] // > 10
    public void Get_Returns_FiveMinutesInterval_For_OtherTypes(string betType, string position, string expected)
    {
        var result = GetPositionName.Get(betType, position);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("abc", "1")]
    [InlineData("221", "xyz")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void Get_Returns_EmptyString_For_InvalidInputs(string betType, string position)
    {
        var result = GetPositionName.Get(betType, position);
        Assert.Equal(string.Empty, result);
    }
}
