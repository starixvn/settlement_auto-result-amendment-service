using ResultAmendment.Features.SetResultsStrategy;
using ResultAmendment.Models;
namespace ResultAmendment.UnitTests.Features;


public class ResultInfoHelpersTests
{
    [Fact]
    public void GetTeamIds_ShouldReturnHomeId_WhenValueIsH()
    {
        // Arrange
        var value = "H";
        var resourceItem = new ResourceItem { HomeId = "101", AwayId = "202" };

        // Act
        var result = ResultInfoHelpers.GetTeamIds(value, resourceItem);

        // Assert
        Assert.Equal("101", result);
    }

    [Fact]
    public void GetTeamIds_ShouldReturnAwayId_WhenValueIsA()
    {
        // Arrange
        var value = "A";
        var resourceItem = new ResourceItem { HomeId = "101", AwayId = "202" };

        // Act
        var result = ResultInfoHelpers.GetTeamIds(value, resourceItem);

        // Assert
        Assert.Equal("202", result);
    }

    [Fact]
    public void GetTeamIds_ShouldReturnEmptyString_WhenValueIsNotHOrA()
    {
        // Arrange
        var value = "X";
        var resourceItem = new ResourceItem { HomeId = "101", AwayId = "202" };

        // Act
        var result = ResultInfoHelpers.GetTeamIds(value, resourceItem);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void NormalizeHomeValue_ShouldReturnNone_WhenHomeValueIsN()
    {
        // Arrange
        var homeValue = "N";

        // Act
        var result = ResultInfoHelpers.NormalizeHomeValue(homeValue);

        // Assert
        Assert.Equal("None", result);
    }

    [Fact]
    public void NormalizeHomeValue_ShouldReturnOriginalValue_WhenHomeValueIsNotN()
    {
        // Arrange
        var homeValue = "H";

        // Act
        var result = ResultInfoHelpers.NormalizeHomeValue(homeValue);

        // Assert
        Assert.Equal("H", result);
    }

    [Fact]
    public void FormatTimeValue_ShouldReturnEmptyString_WhenValueIsNullOrEmpty()
    {
        // Arrange
        var value = string.Empty;

        // Act
        var result = ResultInfoHelpers.FormatTimeValue(value);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void FormatTimeValue_ShouldFormatNegativeTimeValue()
    {
        // Arrange
        var value = "-0";

        // Act
        var result = ResultInfoHelpers.FormatTimeValue(value);

        // Assert
        Assert.Equal("-00", result);
    }

    [Fact]
    public void FormatTimeValue_ShouldFormatPositiveTimeValue()
    {
        // Arrange
        var value = "5";

        // Act
        var result = ResultInfoHelpers.FormatTimeValue(value);

        // Assert
        Assert.Equal("05", result);
    }
}
