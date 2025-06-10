using System.Globalization;
using System.Xml.Serialization;

namespace ResultAmendment.Models;

[XmlRoot("root")]
public class ResultInfo
{
    [XmlElement("r")]
    public List<ResultItem> ResultItems { get; set; }

    [XmlAttribute("type")]
    public string Type { get; set; }
}

public class ResultItem
{
    private string home;
    private string away;

    [XmlAttribute("Id")]
    public string ResourceId { get; set; }

    [XmlAttribute("Home")]
    public string Home
    {
        get => home;
        set => home = (value ?? string.Empty).ToUpper(CultureInfo.InvariantCulture);
    }

    [XmlAttribute("Away")]
    public string Away
    {
        get => away;
        set => away = (value ?? string.Empty).ToUpper(CultureInfo.InvariantCulture);
    }

    [XmlAttribute("Pos")]
    public string Position { get; set; }

    [XmlAttribute("RefBettypeID")]
    public string RefBetTypeId { get; set; }

    [XmlAttribute("X")]
    public string SetX { get; set; }

    [XmlAttribute("Y")]
    public string PointY { get; set; }

    [XmlAttribute("TeamIds")]
    public string TeamIds { get; set; }

    [XmlAttribute("OtherInfo")]
    public string OtherInfo { get; set; }

    [XmlAttribute("O")]
    public string Ordinal { get; set; }

    [XmlAttribute("GroupBetType")]
    public string GroupBetType { get; set; }

    [XmlAttribute("Action")]
    public string Action { get; set; }

    [XmlAttribute("MarketId")]
    public string MarketId { get; set; }
}