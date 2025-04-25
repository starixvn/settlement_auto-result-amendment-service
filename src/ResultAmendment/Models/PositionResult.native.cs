using System.Xml.Serialization;

public class PositionResult
{
    [XmlAttribute("Pos")]
    public string Position { get; set; }

    [XmlAttribute("Home")]
    public string Home { get; set; }

    [XmlAttribute("Away")]
    public string Away { get; set; }

    [XmlAttribute("Recalc")]
    public string ReCalcDateTime { get; set; }

    [XmlAttribute("Calc")]
    public string CalcDateTime { get; set; }
}


