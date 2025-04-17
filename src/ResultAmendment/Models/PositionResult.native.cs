using System.Xml.Serialization;

public class PositionResult
{
    [XmlAttribute("Pos")]
    public string Position { get; set; }

    [XmlAttribute("Home")]
    public string Home { get; set; }

    [XmlAttribute("Away")]
    public string Away { get; set; }
}


