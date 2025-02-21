using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Features.SetResultsStrategy.Strategies;

namespace ResultAmendment.UnitTests.Features.Strategies;
public class MainResultStrategyTests
{
    private readonly MainResultStrategy _strategy;

    public MainResultStrategyTests()
    {
        _strategy = new MainResultStrategy();
    }

    [Fact]
    public void Generate_ShouldReturnCorrectResults_WhenInputsAreValid()
    {
        // Arrange
        var resultChanged = new ResultChanged
        {
            ResourceId = "Res123",
            HomeValue = "1",
            AwayValue = "2"
        };

        var resourceItem = new ResourceItem
        {
            ResourceId = "Res456",
            HomeValue = "2",
            AwayValue = "2"
        };

        // Act
        var (wrongResult, actualResult) = _strategy.Generate(resultChanged, resourceItem);

        // Assert
        Assert.NotNull(wrongResult);
        Assert.Equal(resultChanged.ResourceId, wrongResult.ResourceId);
        Assert.Equal(resultChanged.HomeValue, wrongResult.Home);
        Assert.Equal(resultChanged.AwayValue, wrongResult.Away);
        Assert.Equal(resultChanged.ResourceId, wrongResult.RefBetTypeId);

        Assert.NotNull(actualResult);
        Assert.Equal(resourceItem.ResourceId, actualResult.ResourceId);
        Assert.Equal(resourceItem.HomeValue, actualResult.Home);
        Assert.Equal(resourceItem.AwayValue, actualResult.Away);
        Assert.Equal(resourceItem.ResourceId, actualResult.RefBetTypeId);
    }

    [Fact]
    public void Generate_ShouldThrowArgumentNullException_WhenResultChangedIsNull()
    {
        // Arrange
        var resourceItem = new ResourceItem
        {
            ResourceId = "Res456",
            HomeValue = "1",
            AwayValue = "2"
        };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _strategy.Generate(null, resourceItem));
    }

    [Fact]
    public void Generate_ShouldThrowArgumentNullException_WhenResourceItemIsNull()
    {
        // Arrange
        var resultChanged = new ResultChanged
        {
            ResourceId = "Res123",
            HomeValue = "1",
            AwayValue = "0"
        };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _strategy.Generate(resultChanged, null));
    }
}
