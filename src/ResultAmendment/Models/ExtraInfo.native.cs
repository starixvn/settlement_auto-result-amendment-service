using System.Xml.Serialization;

namespace ResultAmendment.Models;

[XmlRoot("root")]
public class ExtraInfo
{
    [XmlElement("r")]
    public List<ExtraInfoItem> ExtraInfoItems { get; set; }

    [XmlAttribute("type")]
    public string Type { get; set; }

}

public class ExtraInfoItem
{
    [XmlAttribute("Id")]
    public string ResourceId { get; set; }

    [XmlAttribute("ReCalc")]
    public string ReCalcDateTime { get; set; }

    [XmlAttribute("Calc")]
    public string CalcDateTime { get; set; }

    [XmlAttribute("Pos")]
    public string Position { get; set; }

    [XmlAttribute("PosName")]
    public string PositionName { get; set; }
}