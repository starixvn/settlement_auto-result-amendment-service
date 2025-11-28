using Microsoft.Extensions.Options;
using NSubstitute;
using SettlementResultHistory.Constants;
using SettlementResultHistory.Models;
using SettlementResultHistory.Services;
using Shared;

namespace SettlementResultHistory.UnitTests.Services;

public class SettlementResultHistoryServiceTests
{
    private readonly SettlementResultHistoryRepository resultHistoryRepository;
    private readonly IOptions<AppSettings> appSettings;
    private readonly SettlementResultHistoryService service;

    public SettlementResultHistoryServiceTests()
    {
        resultHistoryRepository = Substitute.For<SettlementResultHistoryRepository>();
        var settings = new AppSettings
        {
            EmailAddress = ["test@example.com", "test2@example.com"],
            EnvironmentName = "TEST"
        };
        appSettings = Options.Create(settings);
        service = new SettlementResultHistoryServiceImp(resultHistoryRepository, appSettings);
    }

    [Fact]
    public async Task Handle_WhenNoItems_ShouldNotProcessAnything()
    {
        // Arrange
        resultHistoryRepository.GetResultHistoryByEnv().Returns(new List<ResultHistorySettled>());

        // Act
        await service.Handle();

        // Assert
        await resultHistoryRepository.Received(1).GetResultHistoryByEnv();
        await resultHistoryRepository.DidNotReceive().InsResultHistoryTicketDetails(Arg.Any<ResultHistorySettled>(), Arg.Any<string>());
        await resultHistoryRepository.DidNotReceive().UpdateResultHistory(Arg.Any<ResultHistorySettled>(), Arg.Any<short>());
    }

    [Fact]
    public async Task Handle_WhenItemHasError_ShouldUpdateToErrorStatus()
    {
        // Arrange
        var item = new ResultHistorySettled
        {
            MatchId = 123,
            SessionId = "session1",
            HasError = true,
            DbServer = "dbserver1"
        };

        resultHistoryRepository.GetResultHistoryByEnv().Returns(new List<ResultHistorySettled> { item });

        // Act
        await service.Handle();

        // Assert
        await resultHistoryRepository.Received(1).InsResultHistoryTicketDetails(
            Arg.Is<ResultHistorySettled>(x => x.MatchId == 123 && x.HasError),
            string.Empty);
        await resultHistoryRepository.Received(1).UpdateResultHistory(
            Arg.Is<ResultHistorySettled>(x => x.MatchId == 123),
            ResultHistoryStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenItemIsSuccessful_ShouldCallHandleTicketSettleDone()
    {
        // Arrange
        var item = new ResultHistorySettled
        {
            MatchId = 123,
            SessionId = "session1",
            HasError = false,
            IsCalculated = true,
            IsDoneBalNeg = true,
            DbServer = "dbserver1",
            CreatedDate = DateTime.Now,
            ActualResult = "result1"
        };

        resultHistoryRepository.GetResultHistoryByEnv().Returns(new List<ResultHistorySettled> { item });
        resultHistoryRepository.CheckResultHistoryError(item).Returns(new List<FailureResultHistory>());

        // Act
        await service.Handle();

        // Assert
        await resultHistoryRepository.Received(1).CheckResultHistoryError(item);
        await resultHistoryRepository.Received(1).InsResultHistoryTicketDetails(
            Arg.Is<ResultHistorySettled>(x => x.MatchId == 123),
            string.Empty);
        await resultHistoryRepository.Received(1).UpdateResultHistory(
            Arg.Is<ResultHistorySettled>(x => x.MatchId == 123),
            ResultHistoryStatus.Success);
    }

    [Fact]
    public async Task Handle_WhenItemIsSuccessfulWithErrors_ShouldUpdateToErrorHandled()
    {
        // Arrange
        var item = new ResultHistorySettled
        {
            MatchId = 123,
            SessionId = "session1",
            HasError = false,
            IsCalculated = true,
            IsDoneBalNeg = true,
            DbServer = "dbserver1",
            CreatedDate = DateTime.Now,
            ActualResult = "result1"
        };

        var errorResults = new List<FailureResultHistory>
        {
            new() { SessionId = "error1" },
            new() { SessionId = "error2" }
        };

        resultHistoryRepository.GetResultHistoryByEnv().Returns(new List<ResultHistorySettled> { item });
        resultHistoryRepository.CheckResultHistoryError(item).Returns(errorResults);

        // Act
        await service.Handle();

        // Assert
        await resultHistoryRepository.Received(1).CheckResultHistoryError(item);
        await resultHistoryRepository.Received(1).InsResultHistoryTicketDetails(
            Arg.Is<ResultHistorySettled>(x => x.MatchId == 123),
            "error1,error2");
        await resultHistoryRepository.Received(1).UpdateResultHistory(
            Arg.Is<ResultHistorySettled>(x => x.MatchId == 123),
            ResultHistoryStatus.Success);
        await resultHistoryRepository.Received(1).UpdateResultHistory(
            Arg.Is<ResultHistorySettled>(x => x.MatchId == 123 && x.SessionId == "error1,error2"),
            ResultHistoryStatus.ErrorHandled);
    }

    [Fact]
    public async Task Handle_WhenSuccessfulItemHasEmptyErrorList_ShouldNotUpdateToErrorHandled()
    {
        // Arrange
        var item = new ResultHistorySettled
        {
            MatchId = 123,
            SessionId = "session1",
            HasError = false,
            IsCalculated = true,
            IsDoneBalNeg = true,
            DbServer = "dbserver1",
            CreatedDate = DateTime.Now,
            ActualResult = "result1"
        };

        resultHistoryRepository.GetResultHistoryByEnv().Returns(new List<ResultHistorySettled> { item });
        resultHistoryRepository.CheckResultHistoryError(item).Returns(new List<FailureResultHistory>());

        // Act
        await service.Handle();

        // Assert
        await resultHistoryRepository.Received(1).UpdateResultHistory(
            Arg.Is<ResultHistorySettled>(x => x.MatchId == 123),
            ResultHistoryStatus.Success);
        await resultHistoryRepository.DidNotReceive().UpdateResultHistory(
            Arg.Any<ResultHistorySettled>(),
            ResultHistoryStatus.ErrorHandled);
    }

    [Fact]
    public async Task Handle_WhenSuccessfulItemHasSingleError_ShouldFormatSessionIdCorrectly()
    {
        // Arrange
        var item = new ResultHistorySettled
        {
            MatchId = 123,
            SessionId = "session1",
            HasError = false,
            IsCalculated = true,
            IsDoneBalNeg = true,
            DbServer = "dbserver1",
            CreatedDate = DateTime.Now,
            ActualResult = "result1"
        };

        var errorResults = new List<FailureResultHistory>
        {
            new() { SessionId = "single-error" }
        };

        resultHistoryRepository.GetResultHistoryByEnv().Returns(new List<ResultHistorySettled> { item });
        resultHistoryRepository.CheckResultHistoryError(item).Returns(errorResults);

        // Act
        await service.Handle();

        // Assert
        await resultHistoryRepository.Received(1).InsResultHistoryTicketDetails(
            Arg.Is<ResultHistorySettled>(x => x.MatchId == 123),
            "single-error");
        await resultHistoryRepository.Received(1).UpdateResultHistory(
            Arg.Is<ResultHistorySettled>(x => x.MatchId == 123 && x.SessionId == "single-error"),
            ResultHistoryStatus.ErrorHandled);
    }
}