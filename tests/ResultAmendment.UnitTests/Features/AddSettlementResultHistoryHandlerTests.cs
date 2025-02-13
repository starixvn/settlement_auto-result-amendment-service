using Fanex.Data.Repository;
using Fanex.Settlement.Converter.Services;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ResultAmendment.Contracts;
using ResultAmendment.Features.AddSettlementResultHistory;
using ResultAmendment.Features.GetMainResource;
using ResultAmendment.Models;
using Shared;

namespace ResultAmendment.UnitTests.Features;

public class AddSettlementResultHistoryHandlerTests
{
    private readonly IDynamicRepository repository;
    private readonly XmlConvertService xmlConvertService;
    private readonly IGetMainResourceHandler mainResourceHandler;
    private readonly AddSettlementResultHistoryHandler addSettlementResultHistoryHandler;

    public AddSettlementResultHistoryHandlerTests()
    {
        repository = Substitute.For<IDynamicRepository>();
        xmlConvertService = Substitute.For<XmlConvertService>();
        mainResourceHandler = Substitute.For<IGetMainResourceHandler>();
        var appSettingsOptions = Substitute.For<IOptions<AppSettings>>();

        var appSettings = new AppSettings
        {
            SiteDbServer = ["DB1", "DB2", "DBTXN"]
        };
        appSettingsOptions.Value.Returns(appSettings);

        addSettlementResultHistoryHandler = new AddSettlementResultHistoryHandler(
            repository,
            xmlConvertService,
            mainResourceHandler,
            appSettingsOptions);
    }

    [Fact]
    public async Task HandleAsync_ShouldCallRepositoryWithCorrectData()
    {
        // Arrange
        var resultAmended = new ResultAmended(
            12345,
            "Session123",
            2,
            ActionType.CalculateAll,
            [
                new ResourceItem { ResourceId = "Q1", HomeValue = "10", AwayValue = "11" },
                new ResourceItem { ResourceId = "Q2", HomeValue = "11", AwayValue = "12" }
            ],
            [
                new ResultChanged { ResourceId = "Q1", Action = ActionType.RefundAll, LogDate = DateTime.Now },
                new ResultChanged { ResourceId = "Q2", Action = ActionType.RefundAll, LogDate = DateTime.Now }
            ]);

        var mainResourceIds = new List<string> { "Q1", "Q2" };
        mainResourceHandler.HandlerAsync(resultAmended.SportType).Returns(mainResourceIds);

        xmlConvertService.Serialize(Arg.Any<object>()).Returns("SerializedString");

        // Act
        await addSettlementResultHistoryHandler.HandleAsync(resultAmended);

        // Assert
        await mainResourceHandler.Received(1).HandlerAsync(resultAmended.SportType);

        await repository.Received(2).ExecuteAsync(Arg.Any<AddResultHistoryInsCriteria>());
    }

    [Fact]
    public async Task HandleAsync_ShouldNotProcess_WhenActionTypeIsUnsupported()
    {
        // Arrange
        var resultAmended = new ResultAmended(
            12345,
            "Session123",
            2,
            "InvalidAction",
            [
                new ResourceItem { ResourceId = "R1", }
            ],
            [
                new ResultChanged { ResourceId = "R1", Action = "UnknownAction", LogDate = DateTime.Now }
            ]
        );

        mainResourceHandler.HandlerAsync(resultAmended.SportType).Returns(new List<string> { "R1" });

        // Act
        await addSettlementResultHistoryHandler.HandleAsync(resultAmended);

        // Assert
        await repository.DidNotReceive().ExecuteAsync(Arg.Any<AddResultHistoryInsCriteria>());
    }

    [Fact]
    public async Task HandleAsync_ShouldCatchException_WhenRepositoryThrowsError()
    {
        // Arrange
        var resultAmended = new ResultAmended(
            12345,
            "Session123",
            2,
            ActionType.CalculateAll,
            [
                new ResourceItem { ResourceId = "Q1", HomeValue = "10", AwayValue = "11" },
            ],
            [
                new ResultChanged { ResourceId = "Q1", Action = ActionType.RefundAll, LogDate = DateTime.Now },
            ]);

        repository.ExecuteAsync(Arg.Any<AddResultHistoryInsCriteria>()).Throws(new Exception("Test exception"));

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => addSettlementResultHistoryHandler.HandleAsync(resultAmended));

        Assert.NotNull(exception);
    }
}