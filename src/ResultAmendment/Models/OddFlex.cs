using System.Text.Json.Serialization;

namespace ResultAmendment.Models;

public class OddsInfoModel
{
    [JsonPropertyName("pid")]
    public int PlayerId { get; set; }

    [JsonPropertyName("pname")]
    public string PlayerName { get; set; }

    [JsonPropertyName("tid")]
    public int TeamId { get; set; }

    [JsonPropertyName("isH")]
    public bool IsHome { get; set; }

    [JsonPropertyName("s")]
    public List<Statistic> Statistics { get; set; } = [];

    [JsonPropertyName("r")]
    public List<ResultOdds> Results { get; set; } = [];
}

public class Statistic
{
    [JsonPropertyName("t")]
    public string Type { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }
}

public class ResultOdds
{
    [JsonPropertyName("t")]
    public string Type { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }
}
