using ResultAmendment.Models;

namespace ResultAmendment.Features.SetResultsStrategy;

public static class ResultInfoHelpers
{

    public static string GetTeamIds(string value, ResourceItem resourceItem)
    {
        return value switch
        {
            "H" => $"{resourceItem.HomeId}",
            "A" => $"{resourceItem.AwayId}",
            _ => ""
        };
    }

    public static string NormalizeHomeValue(string homeValue) => homeValue == "N" ? "None" : homeValue;

    public static string FormatTimeValue(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        if (value.StartsWith('-'))
        {
            var absoluteValue = value[0].ToString();
            return "-" + int.Parse(absoluteValue).ToString("D2");
        }

        return int.Parse(value).ToString("D2");
    }
}
