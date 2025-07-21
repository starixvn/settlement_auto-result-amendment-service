using Fanex.Settlement.MessageBridge.ResultAmendment;
using ResultAmendment.Features.SetResultsStrategy;

namespace ResultAmendment.UnitTests.Features.SetResultsStrategy;

public class ResultInfoHelpersTests
{
    #region GetTeamIds Tests

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

    [Theory]
    [InlineData("X")]
    [InlineData("D")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("HOME")]
    [InlineData("AWAY")]
    [InlineData("123")]
    public void GetTeamIds_ShouldReturnEmptyString_WhenValueIsNotHOrA(string value)
    {
        // Arrange
        var resourceItem = new ResourceItem { HomeId = "101", AwayId = "202" };

        // Act
        var result = ResultInfoHelpers.GetTeamIds(value, resourceItem);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetTeamIds_ShouldReturnHomeId_WhenHomeIdIsNull()
    {
        // Arrange
        var value = "H";
        var resourceItem = new ResourceItem { HomeId = string.Empty, AwayId = "202" };

        // Act
        var result = ResultInfoHelpers.GetTeamIds(value, resourceItem);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void GetTeamIds_ShouldReturnAwayId_WhenAwayIdIsNull()
    {
        // Arrange
        var value = "A";
        var resourceItem = new ResourceItem { HomeId = "101", AwayId = string.Empty };

        // Act
        var result = ResultInfoHelpers.GetTeamIds(value, resourceItem);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void GetTeamIds_ShouldReturnHomeId_WhenHomeIdIsEmpty()
    {
        // Arrange
        var value = "H";
        var resourceItem = new ResourceItem { HomeId = "", AwayId = "202" };

        // Act
        var result = ResultInfoHelpers.GetTeamIds(value, resourceItem);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void GetTeamIds_ShouldReturnAwayId_WhenAwayIdIsEmpty()
    {
        // Arrange
        var value = "A";
        var resourceItem = new ResourceItem { HomeId = "101", AwayId = "" };

        // Act
        var result = ResultInfoHelpers.GetTeamIds(value, resourceItem);

        // Assert
        Assert.Equal("", result);
    }

    #endregion GetTeamIds Tests

    #region NormalizeHomeValue Tests

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

    [Theory]
    [InlineData("H")]
    [InlineData("A")]
    [InlineData("D")]
    [InlineData("")]
    [InlineData("HOME")]
    [InlineData("AWAY")]
    [InlineData("123")]
    [InlineData("None")]
    public void NormalizeHomeValue_ShouldReturnOriginalValue_WhenHomeValueIsNotN(string homeValue)
    {
        // Act
        var result = ResultInfoHelpers.NormalizeHomeValue(homeValue);

        // Assert
        Assert.Equal(homeValue, result);
    }

    [Fact]
    public void NormalizeHomeValue_ShouldReturnNone_WhenHomeValueIsNull()
    {
        // Arrange
        string homeValue = "N";

        // Act
        var result = ResultInfoHelpers.NormalizeHomeValue(homeValue);

        // Assert
        Assert.Equal("None", result);
    }

    [Fact]
    public void NormalizeHomeValue_ShouldReturnNone_WhenHomeValueIsLowerCaseN()
    {
        // Arrange
        var homeValue = "A";

        // Act
        var result = ResultInfoHelpers.NormalizeHomeValue(homeValue);

        // Assert
        Assert.Equal("A", result);
    }

    #endregion NormalizeHomeValue Tests

    #region FormatTimeValue Tests

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void FormatTimeValue_ShouldReturnEmptyString_WhenValueIsNullOrEmpty(string value)
    {
        // Act
        var result = ResultInfoHelpers.FormatTimeValue(value);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("-0", "-00")]
    [InlineData("-1", "-01")]
    [InlineData("-5", "-05")]
    [InlineData("-10", "-10")]
    [InlineData("-99", "-99")]
    [InlineData("-100", "-100")]
    public void FormatTimeValue_ShouldFormatNegativeTimeValue(string input, string expected)
    {
        // Act
        var result = ResultInfoHelpers.FormatTimeValue(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("0", "00")]
    [InlineData("1", "01")]
    [InlineData("5", "05")]
    [InlineData("10", "10")]
    [InlineData("99", "99")]
    [InlineData("100", "100")]
    public void FormatTimeValue_ShouldFormatPositiveTimeValue(string input, string expected)
    {
        // Act
        var result = ResultInfoHelpers.FormatTimeValue(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("00", "00")]
    [InlineData("01", "01")]
    [InlineData("05", "05")]
    [InlineData("10", "10")]
    [InlineData("99", "99")]
    public void FormatTimeValue_ShouldFormatAlreadyFormattedValues(string input, string expected)
    {
        // Act
        var result = ResultInfoHelpers.FormatTimeValue(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("-00", "-00")]
    [InlineData("-01", "-01")]
    [InlineData("-05", "-05")]
    [InlineData("-10", "-10")]
    [InlineData("-99", "-99")]
    public void FormatTimeValue_ShouldFormatAlreadyFormattedNegativeValues(string input, string expected)
    {
        // Act
        var result = ResultInfoHelpers.FormatTimeValue(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("5")]
    [InlineData("10")]
    [InlineData("99")]
    [InlineData("100")]
    public void FormatTimeValue_ShouldHandleSingleDigitAndMultiDigitNumbers(string input)
    {
        // Act
        var result = ResultInfoHelpers.FormatTimeValue(input);

        // Assert
        Assert.True(result.Length >= 2);
        if (int.Parse(input) < 10)
        {
            Assert.StartsWith("0", result);
        }
    }

    [Theory]
    [InlineData("-0")]
    [InlineData("-1")]
    [InlineData("-5")]
    [InlineData("-10")]
    [InlineData("-99")]
    [InlineData("-100")]
    public void FormatTimeValue_ShouldHandleNegativeSingleDigitAndMultiDigitNumbers(string input)
    {
        // Act
        var result = ResultInfoHelpers.FormatTimeValue(input);

        // Assert
        Assert.StartsWith("-", result);
        Assert.True(result.Length >= 3); // "-" + at least 2 digits
        var absoluteValue = input[1..];
        if (int.Parse(absoluteValue) < 10)
        {
            Assert.Contains("0", result);
        }
    }

    #endregion FormatTimeValue Tests

    #region Edge Cases and Integration Tests

    [Fact]
    public void GetTeamIds_ShouldHandleCaseSensitivity()
    {
        // Arrange
        var resourceItem = new ResourceItem { HomeId = "101", AwayId = "202" };

        // Act & Assert
        Assert.Equal("", ResultInfoHelpers.GetTeamIds("h", resourceItem));
        Assert.Equal("", ResultInfoHelpers.GetTeamIds("a", resourceItem));
        Assert.Equal("101", ResultInfoHelpers.GetTeamIds("H", resourceItem));
        Assert.Equal("202", ResultInfoHelpers.GetTeamIds("A", resourceItem));
    }

    [Fact]
    public void NormalizeHomeValue_ShouldHandleCaseSensitivity()
    {
        // Act & Assert
        Assert.Equal("None", ResultInfoHelpers.NormalizeHomeValue("N"));
        Assert.Equal("n", ResultInfoHelpers.NormalizeHomeValue("n"));
        Assert.Equal("NONE", ResultInfoHelpers.NormalizeHomeValue("NONE"));
    }

    [Fact]
    public void FormatTimeValue_ShouldHandleZeroValues()
    {
        // Act & Assert
        Assert.Equal("00", ResultInfoHelpers.FormatTimeValue("0"));
        Assert.Equal("-00", ResultInfoHelpers.FormatTimeValue("-0"));
    }

    [Fact]
    public void FormatTimeValue_ShouldHandleLargeNumbers()
    {
        // Act & Assert
        Assert.Equal("999", ResultInfoHelpers.FormatTimeValue("999"));
        Assert.Equal("-999", ResultInfoHelpers.FormatTimeValue("-999"));
        Assert.Equal("1000", ResultInfoHelpers.FormatTimeValue("1000"));
        Assert.Equal("-1000", ResultInfoHelpers.FormatTimeValue("-1000"));
    }

    #endregion Edge Cases and Integration Tests
}