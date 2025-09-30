using Fanex.Settlement.Converter.Services;
using Fanex.Settlement.MessageBridge.Constants;
using Fanex.Settlement.MessageBridge.ResultAmendment;
using NSubstitute;
using ResultAmendment.Features.AddSettlementResultHistory;
using ResultAmendment.Models;

namespace ResultAmendment.UnitTests.Features.AddSettlementResultHistory;

public class GenerateResultServiceTests
{
    private readonly XmlConvertService xmlConvertService;
    private readonly GenerateResultService generateResultService;

    public GenerateResultServiceTests()
    {
        xmlConvertService = Substitute.For<XmlConvertService>();
        generateResultService = new GenerateResultService(xmlConvertService);
    }

    [Fact]
    public void GenerateWrongAndActualResults_ShouldReturnCorrectResults_WhenValidInputProvided()
    {
        // Arrange
        var resultsChanged = new List<ResultChanged>
        {
            new() { ResourceId = "Q1", Action = ActionType.CalculateAll, LogDate = DateTime.Now }
        };

        var selectedResources = new List<ResourceItem>
        {
            new() { ResourceId = "Q1", GroupBetType = 1, HomeValue = "1", AwayValue = "2" }
        };

        var mainResourceIds = new List<string> { "Q1" };

        var sportType = 2;

        // Act
        var (wrongResults, actualResults) = generateResultService.GenerateWrongAndActualResults(
            resultsChanged, selectedResources, mainResourceIds, sportType);

        // Assert
        Assert.NotNull(wrongResults);
        Assert.NotNull(actualResults);
        Assert.Single(wrongResults);
        Assert.Single(actualResults);
        Assert.Equal("Q1", wrongResults[0].ResourceId);
        Assert.Equal("Q1", actualResults[0].ResourceId);
    }

    [Fact]
    public void GenerateWrongAndActualResults_ShouldSkipResources_WhenNoMatchingResultChanged()
    {
        // Arrange
        var resultsChanged = new List<ResultChanged>
        {
            new() { ResourceId = "Q1", Action = ActionType.CalculateAll, LogDate = DateTime.Now }
        };

        var selectedResources = new List<ResourceItem>
        {
            new() { ResourceId = "Q1", GroupBetType = 1, HomeValue = "1", AwayValue = "2" },
            new() { ResourceId = "Q2", GroupBetType = 1, HomeValue = "3", AwayValue = "4" }
        };

        var mainResourceIds = new List<string> { "Q1", "Q2" };
        var sportType = 2;

        // Act
        var (wrongResults, actualResults) = generateResultService.GenerateWrongAndActualResults(
            resultsChanged, selectedResources, mainResourceIds, sportType);

        // Assert
        Assert.NotNull(wrongResults);
        Assert.NotNull(actualResults);
        Assert.Single(wrongResults);
        Assert.Single(actualResults);
        Assert.Equal("Q1", wrongResults[0].ResourceId);
        Assert.Equal("Q1", actualResults[0].ResourceId);
    }

    [Fact]
    public void GenerateWrongAndActualResults_ShouldReturnEmptyLists_WhenNoResultsChanged()
    {
        // Arrange
        var resultsChanged = new List<ResultChanged>();

        var selectedResources = new List<ResourceItem>
        {
            new() { ResourceId = "Q1", GroupBetType = 1, HomeValue = "1", AwayValue = "2" }
        };

        var mainResourceIds = new List<string> { "Q1" };

        var sportType = 2;

        // Act
        var (wrongResults, actualResults) = generateResultService.GenerateWrongAndActualResults(
            resultsChanged, selectedResources, mainResourceIds, sportType);

        // Assert
        Assert.NotNull(wrongResults);
        Assert.NotNull(actualResults);
        Assert.Empty(wrongResults);
        Assert.Empty(actualResults);
    }

    [Fact]
    public void GenerateFastMarketResult_ShouldReturnCorrectResults_WhenValidInputProvided()
    {
        // Arrange
        var action = ActionType.CalculateAll;
        var resultChanged = new List<ResultChanged>
        {
            new() { ResourceId = "221", OtherInfo = "5", Action = ActionType.CalculateAll, LogDate = DateTime.Now }
        };

        var selectedResources = new List<ResourceItem>
        {
            new() { ResourceId = "221", OtherInfo = "5", HomeValue = "1", AwayValue = "2" }
        };

        // Act
        var (wrongResults, actualResults) = generateResultService.GenerateFastMarketResult(
            action, resultChanged, selectedResources);

        // Assert
        Assert.NotNull(wrongResults);
        Assert.NotNull(actualResults);
        Assert.Single(wrongResults);
        Assert.Single(actualResults);
        Assert.Equal("221", wrongResults[0].ResourceId);
        Assert.Equal("221", actualResults[0].ResourceId);
    }

    [Fact]
    public void GenerateFastMarketResult_ShouldSkipResources_WhenNoMatchingResultChanged()
    {
        // Arrange
        var action = ActionType.CalculateAll;
        var resultChanged = new List<ResultChanged>
        {
            new() { ResourceId = "221", OtherInfo = "5", Action = ActionType.CalculateAll, LogDate = DateTime.Now }
        };

        var selectedResources = new List<ResourceItem>
        {
            new() { ResourceId = "221", OtherInfo = "5", HomeValue = "1", AwayValue = "2" },
            new() { ResourceId = "221", OtherInfo = "6", HomeValue = "3", AwayValue = "4" }
        };

        // Act
        var (wrongResults, actualResults) = generateResultService.GenerateFastMarketResult(
            action, resultChanged, selectedResources);

        // Assert
        Assert.NotNull(wrongResults);
        Assert.NotNull(actualResults);
        Assert.Single(wrongResults);
        Assert.Single(actualResults);
        Assert.Equal("221", wrongResults[0].ResourceId);
        Assert.Equal("221", actualResults[0].ResourceId);
    }

    [Fact]
    public void GenerateFastMarketResult_ShouldReturnEmptyLists_WhenNoResultsChanged()
    {
        // Arrange
        var action = ActionType.CalculateAll;
        var resultChanged = new List<ResultChanged>();

        var selectedResources = new List<ResourceItem>
        {
            new() { ResourceId = "221", OtherInfo = "5", HomeValue = "1", AwayValue = "2" }
        };

        // Act
        var (wrongResults, actualResults) = generateResultService.GenerateFastMarketResult(
            action, resultChanged, selectedResources);

        // Assert
        Assert.NotNull(wrongResults);
        Assert.NotNull(actualResults);
        Assert.Empty(wrongResults);
        Assert.Empty(actualResults);
    }

    [Fact]
    public void GenerateExtraInfo_ShouldReturnSerializedExtraInfo_WhenValidInputProvided()
    {
        // Arrange
        var selectedResources = new List<ResourceItem>
        {
            new() { ResourceId = "Q1", OtherInfo = "5" }
        };

        var resultsChanged = new List<ResultChanged>
        {
            new() { ResourceId = "Q1", LogDate = DateTime.Now }
        };

        var type = ResultAmendmentType.Normal;
        var expectedSerializedResult = "<root><r Id=\"Q1\" Calc=\"2024-01-01 12:00:00.000\" ReCalc=\"2024-01-01 12:00:00.000\" /></root>";

        xmlConvertService.Serialize(Arg.Any<ExtraInfo>()).Returns(expectedSerializedResult);

        // Act
        var result = generateResultService.GenerateExtraInfo(selectedResources, resultsChanged, type);

        // Assert
        Assert.Equal(expectedSerializedResult, result);
        xmlConvertService.Received(1).Serialize(Arg.Any<ExtraInfo>());
    }

    [Fact]
    public void GenerateExtraInfo_ShouldHandleFastMarketType_WhenTypeIsFastMarket()
    {
        // Arrange
        var selectedResources = new List<ResourceItem>
        {
            new() { ResourceId = "221", OtherInfo = "5" }
        };

        var resultsChanged = new List<ResultChanged>
        {
            new() { ResourceId = "221", OtherInfo = "5", LogDate = DateTime.Now }
        };

        var type = ResultAmendmentType.FastMarket;
        var expectedSerializedResult = "<root><r Id=\"221\" Calc=\"2024-01-01 12:00:00.000\" ReCalc=\"2024-01-01 12:00:00.000\" Pos=\"5\" PosName=\"05:00-05:59\" /></root>";

        xmlConvertService.Serialize(Arg.Any<ExtraInfo>()).Returns(expectedSerializedResult);

        // Act
        var result = generateResultService.GenerateExtraInfo(selectedResources, resultsChanged, type);

        // Assert
        Assert.Equal(expectedSerializedResult, result);
        xmlConvertService.Received(1).Serialize(Arg.Any<ExtraInfo>());
    }

    [Fact]
    public void GenerateExtraInfo_ShouldSkipResources_WhenNoMatchingResultChanged()
    {
        // Arrange
        var selectedResources = new List<ResourceItem>
        {
            new() { ResourceId = "Q1", OtherInfo = "5" },
            new() { ResourceId = "Q2", OtherInfo = "6" }
        };

        var resultsChanged = new List<ResultChanged>
        {
            new() { ResourceId = "Q1", LogDate = DateTime.Now }
        };

        var type = ResultAmendmentType.Normal;
        var expectedSerializedResult = "<root><r Id=\"Q1\" Calc=\"2024-01-01 12:00:00.000\" ReCalc=\"2024-01-01 12:00:00.000\" /></root>";

        xmlConvertService.Serialize(Arg.Any<ExtraInfo>()).Returns(expectedSerializedResult);

        // Act
        var result = generateResultService.GenerateExtraInfo(selectedResources, resultsChanged, type);

        // Assert
        Assert.Equal(expectedSerializedResult, result);
        xmlConvertService.Received(1).Serialize(Arg.Any<ExtraInfo>());
    }

    [Fact]
    public void GenerateExtraInfo_ShouldReturnEmptyExtraInfo_WhenNoResultsChanged()
    {
        // Arrange
        var selectedResources = new List<ResourceItem>
        {
            new() { ResourceId = "Q1", OtherInfo = "5" }
        };

        var resultsChanged = new List<ResultChanged>();

        var type = ResultAmendmentType.Normal;
        var expectedSerializedResult = "<root></root>";

        xmlConvertService.Serialize(Arg.Any<ExtraInfo>()).Returns(expectedSerializedResult);

        // Act
        var result = generateResultService.GenerateExtraInfo(selectedResources, resultsChanged, type);

        // Assert
        Assert.Equal(expectedSerializedResult, result);
        xmlConvertService.Received(1).Serialize(Arg.Any<ExtraInfo>());
    }
}