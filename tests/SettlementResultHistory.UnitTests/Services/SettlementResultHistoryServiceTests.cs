using Microsoft.Extensions.Options;
using NSubstitute;
using SettlementResultHistory.Constants;
using SettlementResultHistory.Models;
using SettlementResultHistory.Services;
using Shared;

namespace SettlementResultHistory.UnitTests.Services;

// AAA (Arrange-Act-Assert) formatted test class
public class SettlementResultHistoryServiceTests
{
    private readonly SettlementResultHistoryRepository _repo;
    private readonly SettlementResultHistoryServiceImp _service;
    private readonly IOptions<AppSettings> _settings;

    public SettlementResultHistoryServiceTests()
    {
        _settings = Options.Create(new AppSettings
        {
            EnvironmentName = "PRO",
            SiteDbServer = ["abc", "xyz"],
            EmailAddress = ["harley.nguyen@starixsoft.com"]
        });

        _repo = Substitute.For<SettlementResultHistoryRepository>();
        _service = new SettlementResultHistoryServiceImp(_repo, _settings);
    }

    [Fact]
    public async Task Handle_WhenItemsPresent_ProcessesEach() // AAA
    {
        // Arrange
        var items = new List<ResultHistorySettled>
        {
            new() { HasError = false, IsCalculated = true, IsDoneBalNeg = true }
        };
        _repo.GetResultHistoryByEnv().Returns(items);
        _repo.CheckResultHistoryError(Arg.Any<ResultHistorySettled>())
             .Returns([new() { SessionId = "abc" }]);

        // Act
        await _service.Handle();

        // Assert
        await _repo.Received(2)
            .UpdateResultHistory(Arg.Any<ResultHistorySettled>(), Arg.Any<short>());
    }

    [Fact]
    public async Task Handle_WhenItemHasError_SetsErrorStatus() // AAA
    {
        // Arrange
        var items = new List<ResultHistorySettled> { new() { HasError = true } };
        _repo.GetResultHistoryByEnv().Returns(items);

        // Act
        await _service.Handle();

        // Assert
        await _repo.Received(1)
            .InsResultHistoryTicketDetails(Arg.Any<ResultHistorySettled>(), string.Empty);
        await _repo.Received(1)
            .UpdateResultHistory(Arg.Any<ResultHistorySettled>(), ResultHistoryStatus.Error);
    }

    [Fact]
    public async Task Handle_WhenCalculatedAndBalancedWithNoErrors_SetsSuccess() // AAA
    {
        // Arrange
        var item = new ResultHistorySettled { IsCalculated = true, IsDoneBalNeg = true };
        _repo.GetResultHistoryByEnv().Returns(new List<ResultHistorySettled> { item });
        _repo.CheckResultHistoryError(item).Returns(new List<FailureResultHistory>());

        // Act
        await _service.Handle();

        // Assert
        await _repo.Received(1).UpdateResultHistory(item, ResultHistoryStatus.Success);
        await _repo.DidNotReceive()
            .UpdateResultHistory(item, ResultHistoryStatus.ErrorHandled);
    }

    [Fact]
    public async Task Handle_WhenCalculatedAndBalancedWithErrors_SetsErrorHandled() // AAA
    {
        // Arrange
        var item = new ResultHistorySettled { IsCalculated = true, IsDoneBalNeg = true };
        _repo.GetResultHistoryByEnv().Returns(new List<ResultHistorySettled> { item });
        _repo.CheckResultHistoryError(item).Returns(new List<FailureResultHistory>
        {
            new() { SessionId = "S1" }, new() { SessionId = "S2" }
        });

        // Act
        await _service.Handle();

        // Assert
        await _repo.Received(1)
            .InsResultHistoryTicketDetails(item, "S1,S2");
        await _repo.Received(1)
            .UpdateResultHistory(item, ResultHistoryStatus.Success);
        await _repo.Received(1)
            .UpdateResultHistory(item, ResultHistoryStatus.ErrorHandled);
    }
}