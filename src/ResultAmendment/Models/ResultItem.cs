using System.Xml.Serialization;

namespace ResultAmendment.Models;

[XmlRoot("root")]
public class ResultInfo
{
    [XmlElement("r")]
    public List<ResultItem> ResultItems { get; set; }
}


public class ResultItem
{
    [XmlAttribute("Id")]
    public string ResourceId { get; set; }

    [XmlAttribute("Home")]
    public string Home { get; set; }

    [XmlAttribute("Away")]
    public string Away { get; set; }

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
}
