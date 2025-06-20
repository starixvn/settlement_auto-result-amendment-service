using Microsoft.Extensions.Options;
using NSubstitute;
using SettlementResultHistory.Constants;
using SettlementResultHistory.Models;
using SettlementResultHistory.Services;
using Shared;
using MailAppSettings = Fanex.MailDeliver.MailClient.AppSettings;

namespace SettlementResultHistory.UnitTests.Services;

public class SettlementResultHistoryServiceTests
{
    private readonly SettlementResultHistoryRepository resultHistoryRepository;
    private readonly SettlementResultHistoryServiceImp service;

    public SettlementResultHistoryServiceTests()
    {
        var settings = new AppSettings()
        {
            EnvironmentName = "PRO",
            SiteDbServer = ["abc", "xyz"],
            EmailAddress = ["harley.nguyen@starixsoft.com"]
        };

        var appSettingsOptions = Options.Create(settings);
        resultHistoryRepository = Substitute.For<SettlementResultHistoryRepository>();
        service = new SettlementResultHistoryServiceImp(resultHistoryRepository, appSettingsOptions);
    }

    [Fact]
    public async Task Handle_ShouldProcessEachResultHistoryItem()
    {
        // Arrange
        var resultHistoryItems = new List<ResultHistorySettled>
        {
            new() { HasError = false, IsCalculated = true, IsDoneBalNeg = true },
        };
        resultHistoryRepository.GetResultHistoryByEnv().Returns(resultHistoryItems);
        resultHistoryRepository.CheckResultHistoryError(Arg.Any<ResultHistorySettled>()).Returns([
            new() { SessionId = "abc" }
        ]);

        // Act
        await service.Handle();

        // Assert
        await resultHistoryRepository.Received(2).UpdateResultHistory(Arg.Any<ResultHistorySettled>(), Arg.Any<short>());
    }

    [Fact]
    public async Task Handle_ShouldHandleErrorResultHistory()
    {
        // Arrange
        var resultHistoryItems = new List<ResultHistorySettled>
        {
            new () { HasError = true },
        };
        resultHistoryRepository.GetResultHistoryByEnv().Returns(resultHistoryItems);
        resultHistoryRepository.CheckResultHistoryError(Arg.Any<ResultHistorySettled>()).Returns([
            new() { SessionId = "abc" }
        ]);

        // Act
        await service.Handle();

        // Assert
        await resultHistoryRepository.Received(1).InsResultHistoryTicketDetails(Arg.Any<ResultHistorySettled>(), Arg.Any<string>());
        await resultHistoryRepository.Received(1).UpdateResultHistory(Arg.Any<ResultHistorySettled>(), ResultHistoryStatus.Error);
    }

    [Fact]
    public async Task Process_ShouldHandleSuccessfulResultHistory()
    {
        // Arrange
        var item = new ResultHistorySettled { HasError = false, IsCalculated = false, IsDoneBalNeg = false };
        var resultHistoryItems = new List<ResultHistorySettled>
        {
            item
        };
        resultHistoryRepository.GetResultHistoryByEnv().Returns(resultHistoryItems);
        resultHistoryRepository.CheckResultHistoryError(Arg.Any<ResultHistorySettled>()).Returns([
            new() { SessionId = "abc" }
        ]);
        MailAppSettings.AppMailId = "1";
        MailAppSettings.AppMailAddress = "http://mail_service.nexdev.net:8787/";
        // Act
        await service.Handle();

        // Assert
        await resultHistoryRepository.Received(1).InsResultHistoryTicketDetails(item, Arg.Any<string>());
        await resultHistoryRepository.Received(1).UpdateResultHistory(Arg.Any<ResultHistorySettled>(), Arg.Any<short>());
        resultHistoryRepository.Received(1).InsLetsTalkMessage(Arg.Any<ResultHistorySettled>());
    }
}