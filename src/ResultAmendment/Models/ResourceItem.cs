using System.Xml.Serialization;

namespace ResultAmendment.Models;
public class BaseResourceItem
{
    [XmlAttribute("Id")]
    public string ResourceId { get; set; }

    [XmlAttribute("Home")]
    public string HomeValue { get; set; }

    [XmlAttribute("Away")]
    public string AwayValue { get; set; }
}

public class ResourceItem : BaseResourceItem
{
    private string determined = "0";

    public ResourceItem()
    {
    }

    public ResourceItem(string id)
    {
        ResourceId = id;
    }

    [XmlAttribute("OtherInfo")]
    public string OtherInfo { get; set; }

    [XmlAttribute("IsDetermined")]
    public string Determined
    {
        get => IsSettleDetermined ? "1" : determined;
        set => determined = value;
    }

    [XmlIgnore]
    public bool IsSettleDetermined { get; set; }

    [XmlIgnore]
    public string AwayId { get; set; }

    [XmlIgnore]
    public string HomeId { get; set; }

    [XmlIgnore]
    public string ResourceName { get; set; }

    [XmlIgnore]
    public int GroupBetType { get; set; }
}