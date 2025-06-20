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
    public void GenerateWrongAndActualResults_ShouldReturnCorrectResults_WhenMainResultStrategyIsUsed()
    {
        // Arrange
        var resultsChanged = new List<ResultChanged>
        {
            new() { ResourceId = "00", HomeValue = "1", AwayValue = "0" }
        };
        var selectedResources = new List<ResourceItem>
        {
            new() { ResourceId = "00", HomeValue = "2", AwayValue = "1", GroupBetType = 0 }
        };
        var mainResourceIds = new List<string> { "00" };

        // Act
        var (wrongResults, actualResults) = generateResultService.GenerateWrongAndActualResults(resultsChanged, selectedResources, mainResourceIds);

        // Assert
        Assert.Single(wrongResults);
        var wrongResult = wrongResults.First();
        Assert.Equal("00", wrongResult.ResourceId);
        Assert.Equal("1", wrongResult.Home);
        Assert.Equal("0", wrongResult.Away);

        Assert.Single(actualResults);
        var actualResult = actualResults.First();
        Assert.Equal("00", actualResult.ResourceId);
        Assert.Equal("2", actualResult.Home);
        Assert.Equal("1", actualResult.Away);
    }

    [Fact]
    public void GenerateWrongAndActualResults_ShouldSkipItemsNotInResultsChanged()
    {
        // Arrange
        var resultsChanged = new List<ResultChanged>
        {
            new() { ResourceId = "00", HomeValue = "1", AwayValue = "0" }
        };
        var selectedResources = new List<ResourceItem>
        {
            new() { ResourceId = "01", HomeValue = "2", AwayValue = "1", GroupBetType = 0 }
        };
        var mainResourceIds = new List<string> { "00" };

        // Act
        var (wrongResults, actualResults) = generateResultService.GenerateWrongAndActualResults(resultsChanged, selectedResources, mainResourceIds);

        // Assert
        Assert.Empty(wrongResults);
        Assert.Empty(actualResults);
    }

    [Fact]
    public void GenerateFastMarketResult_ShouldReturnCorrectResults_WhenMatchIsFound()
    {
        // Arrange
        var action = "Calculate";
        var resultsChanged = new List<ResultChanged>
        {
            new() { ResourceId = "221", OtherInfo = "5|some_data", HomeValue = "1", AwayValue = "0", Action = "Initial" }
        };
        var selectedResources = new List<ResourceItem>
        {
            new() { ResourceId = "221", OtherInfo = "5|some_data", HomeValue = "2", AwayValue = "1" }
        };

        // Act
        var (wrongResults, actualResults) = generateResultService.GenerateFastMarketResult(action, resultsChanged, selectedResources);

        // Assert
        Assert.Single(wrongResults);
        var wrongResult = wrongResults.First();
        Assert.Equal("221", wrongResult.ResourceId);
        Assert.Equal("5", wrongResult.Position);
        Assert.Equal("1", wrongResult.Home);
        Assert.Equal("0", wrongResult.Away);
        Assert.Equal("Initial", wrongResult.Action);

        Assert.Single(actualResults);
        var actualResult = actualResults.First();
        Assert.Equal("221", actualResult.ResourceId);
        Assert.Equal("5", actualResult.Position);
        Assert.Equal("2", actualResult.Home);
        Assert.Equal("1", actualResult.Away);
        Assert.Equal("Calculate", actualResult.Action);
    }

    [Fact]
    public void GenerateFastMarketResult_ShouldSkipItemsNotInResultsChanged()
    {
        // Arrange
        var action = "Calculate";
        var resultsChanged = new List<ResultChanged>
        {
            new() { ResourceId = "221", OtherInfo = "5", HomeValue = "1", AwayValue = "0" }
        };
        var selectedResources = new List<ResourceItem>
        {
            new() { ResourceId = "222", OtherInfo = "6", HomeValue = "2", AwayValue = "1" } // Different ResourceId
        };

        // Act
        var (wrongResults, actualResults) = generateResultService.GenerateFastMarketResult(action, resultsChanged, selectedResources);

        // Assert
        Assert.Empty(wrongResults);
        Assert.Empty(actualResults);
    }

    [Fact]
    public void GenerateBetBuilderResult_ShouldReturnCorrectResults_WhenMatchIsFound()
    {
        // Arrange
        var resultsChanged = new List<ResultChanged>
        {
            new() { ResourceId = "BB1", OtherInfo = "Market123", HomeValue = "1", AwayValue = "0", Action = "Initial" }
        };
        var selectedResources = new List<ResourceItem>
        {
            new() { ResourceId = "BB1", OtherInfo = "Market123", HomeValue = "2", AwayValue = "1", Action = "Settled" }
        };

        // Act
        var (wrongResults, actualResults) = generateResultService.GenerateBetBuilderResult(resultsChanged, selectedResources);

        // Assert
        Assert.Single(wrongResults);
        var wrongResult = wrongResults.First();
        Assert.Equal("BB1", wrongResult.ResourceId);
        Assert.Equal("1", wrongResult.Home);
        Assert.Equal("0", wrongResult.Away);
        Assert.Equal("Initial", wrongResult.Action);
        Assert.Equal("Market123", wrongResult.MarketId);

        Assert.Single(actualResults);
        var actualResult = actualResults.First();
        Assert.Equal("BB1", actualResult.ResourceId);
        Assert.Equal("2", actualResult.Home);
        Assert.Equal("1", actualResult.Away);
        Assert.Equal("Settled", actualResult.Action);
        Assert.Equal("Market123", actualResult.MarketId);
    }

    [Fact]
    public void GenerateBetBuilderResult_ShouldSkipItemsNotInResultsChanged()
    {
        // Arrange
        var resultsChanged = new List<ResultChanged>
        {
            new() { ResourceId = "BB1", OtherInfo = "Market123" }
        };
        var selectedResources = new List<ResourceItem>
        {
            new() { ResourceId = "BB2", OtherInfo = "Market456" }
        };

        // Act
        var (wrongResults, actualResults) = generateResultService.GenerateBetBuilderResult(resultsChanged, selectedResources);

        // Assert
        Assert.Empty(wrongResults);
        Assert.Empty(actualResults);
    }

    [Fact]
    public void GenerateExtraInfo_ForFastMarket_ShouldGenerateCorrectXml()
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

        xmlConvertService.Serialize(Arg.Any<ExtraInfo>())
            .Returns(callInfo =>
            {
                var ei = callInfo.Arg<ExtraInfo>();
                Assert.Single(ei.ExtraInfoItems);
                var item = ei.ExtraInfoItems.First();
                Assert.Equal("221", item.ResourceId);
                Assert.Equal("5", item.Position);
                Assert.NotNull(item.PositionName);
                Assert.Null(item.MarketId);
                return "xml_string";
            });

        // Act
        var result = generateResultService.GenerateExtraInfo(selectedResources, resultsChanged, type);

        // Assert
        Assert.Equal("xml_string", result);
        xmlConvertService.Received(1).Serialize(Arg.Any<ExtraInfo>());
    }

    [Fact]
    public void GenerateExtraInfo_ForBetBuilder_ShouldGenerateCorrectXml()
    {
        // Arrange
        var selectedResources = new List<ResourceItem>
        {
            new() { ResourceId = "BB1", OtherInfo = "Market123" }
        };
        var resultsChanged = new List<ResultChanged>
        {
            new() { ResourceId = "BB1", OtherInfo = "Market123", LogDate = DateTime.Now }
        };
        var type = "BB";

        xmlConvertService.Serialize(Arg.Any<ExtraInfo>())
            .Returns(callInfo =>
            {
                var ei = callInfo.Arg<ExtraInfo>();
                Assert.Single(ei.ExtraInfoItems);
                var item = ei.ExtraInfoItems.First();
                Assert.Equal("BB1", item.ResourceId);
                Assert.Null(item.Position);
                Assert.Null(item.PositionName);
                Assert.Equal("Market123", item.MarketId);
                return "xml_string";
            });

        // Act
        var result = generateResultService.GenerateExtraInfo(selectedResources, resultsChanged, type);

        // Assert
        Assert.Equal("xml_string", result);
        xmlConvertService.Received(1).Serialize(Arg.Any<ExtraInfo>());
    }

    [Fact]
    public void GenerateExtraInfo_ForStandardType_ShouldGenerateCorrectXml()
    {
        // Arrange
        var selectedResources = new List<ResourceItem>
        {
            new() { ResourceId = "123" }
        };
        var resultsChanged = new List<ResultChanged>
        {
            new() { ResourceId = "123", LogDate = DateTime.Now }
        };
        var type = "Standard";

        xmlConvertService.Serialize(Arg.Any<ExtraInfo>())
           .Returns(callInfo =>
           {
               var ei = callInfo.Arg<ExtraInfo>();
               Assert.Single(ei.ExtraInfoItems);
               var item = ei.ExtraInfoItems.First();
               Assert.Equal("123", item.ResourceId);
               Assert.Null(item.Position);
               Assert.Null(item.PositionName);
               Assert.Null(item.MarketId);
               return "xml_string";
           });

        // Act
        var result = generateResultService.GenerateExtraInfo(selectedResources, resultsChanged, type);

        // Assert
        Assert.Equal("xml_string", result);
        xmlConvertService.Received(1).Serialize(Arg.Any<ExtraInfo>());
    }
}