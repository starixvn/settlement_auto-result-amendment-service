using Fanex.Caching;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using NSubstitute;
using SettlementResultHistory.Criteria;
using SettlementResultHistory.Models;
using SettlementResultHistory.Services;
using Shared;

namespace SettlementResultHistory.UnitTests.Services;

public class SettlementResultHistoryRepositoryTests
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly SettlementResultHistoryRepository repository;

    public SettlementResultHistoryRepositoryTests()
    {
        var settings = new AppSettings()
        {
            EnvironmentName = "PRO",
            SiteDbServer = ["abc", "xyz"]
        };

        var appSettingsOptions = Options.Create(settings);
        dynamicRepository = Substitute.For<IDynamicRepository>();
        var cache = Substitute.For<ICache>();
        repository = new SettlementResultHistoryRepositoryImp(appSettingsOptions, dynamicRepository, cache);
    }

    [Fact]
    public async Task GetResultHistoryByEnv_WhenPROEnvironment_ReturnsFilteredResult()
    {
        // Arrange
        var resultHistory = new List<ResultHistorySettled>
        {
            new() { LeagueId = 1 },
            new() { LeagueId = 2 },
            new() { LeagueId = 3 },
            new() { LeagueId = 4 },
            new() { LeagueId = 5 }
        };

        dynamicRepository.FetchAsync<ResultHistorySettled>(Arg.Any<SettlementResultHistoryCriteria>()).Returns(resultHistory);

        // Act
        var actualResult = await repository.GetResultHistoryByEnv();

        // Assert
        Assert.Equal(10, actualResult.Count);
        Assert.Contains(actualResult, x => x.LeagueId == 4);
        Assert.Contains(actualResult, x => x.LeagueId == 5);
    }

    [Fact]
    public async Task UpdateResultHistory_ShouldExecuteDynamicRepositoryWithCorrectCriteria()
    {
        // Arrange
        var item = new ResultHistorySettled
        {
            SessionId = "session123",
            StatusId = 1,
            MatchId = 123,
            DbServer = "dbserver"
        };

        // Act
        await repository.UpdateResultHistory(item, item.StatusId);

        // Assert
        await dynamicRepository.Received(1).ExecuteAsync(Arg.Any<ResultHistoryUpdateCriteria>());
    }

    [Fact]
    public async Task InsResultHistoryTicketDetails_ShouldCallDynamicRepositoryExecuteAsync()
    {
        // Arrange
        var item = new ResultHistorySettled();
        var errorSessionIds = "12345";

        // Act
        await repository.InsResultHistoryTicketDetails(item, errorSessionIds);

        // Assert
        await dynamicRepository.Received(1).ExecuteAsync(Arg.Any<ResultHistoryTicketDetailInsCriteria>());
    }

    [Fact]
    public async Task GetResultHistoryByEnv_ShouldReturnOrderedResult()
    {
        // Arrange
        var resultHistory = new List<ResultHistorySettled>
        {
            new() { LeagueId = 1, CreatedDate = new DateTime(2022, 1, 1) },
            new() { LeagueId = 2, CreatedDate = new DateTime(2022, 1, 2) },
            new() { LeagueId = 3, CreatedDate = new DateTime(2022, 1, 3) }
        };

        dynamicRepository.FetchAsync<ResultHistorySettled>(Arg.Any<SettlementResultHistoryCriteria>()).Returns(resultHistory);

        // Act
        var actualResult = await repository.GetResultHistoryByEnv();

        // Assert
        Assert.Equal(6, actualResult.Count);
    }

    [Fact]
    public void InsLetsTalkMessage_EnvironmentPRO_ShouldExecuteDynamicRepository()
    {
        // Arrange
        var item = new ResultHistorySettled
        {
            DbServer = "TestDbServer",
            MatchId = 1,
            SessionId = "TestSessionId"
        };

        // Act
        repository.InsLetsTalkMessage(item);

        // Assert
        dynamicRepository.Received(1).Execute(Arg.Any<LetstalkMessageInsCriteria>());
    }

    [Fact]
    public async Task CheckResultHistoryError_ShouldReturnFailureResultHistoryList()
    {
        // Arrange
        var item = new ResultHistorySettled
        {
            SessionId = "session123",
            CreatedDate = new DateTime(2022, 1, 1),
            ActualResult = "result",
            MatchId = 123,
            DbServer = "server"
        };

        var criteria = Arg.Is<ResultHistoryErrorCheckCriteria>(c =>
            c.SessionId == item.SessionId &&
            c.CreatedDate == item.CreatedDate &&
            c.ActualResult == item.ActualResult &&
            c.MatchId == item.MatchId &&
            c.DbServer == item.DbServer);

        var expectedResult = new List<FailureResultHistory>();
        dynamicRepository.FetchAsync<FailureResultHistory>(criteria).Returns(expectedResult);

        // Act
        var result = await repository.CheckResultHistoryError(item);

        // Assert
        Assert.Equal(expectedResult, result);
    }
}