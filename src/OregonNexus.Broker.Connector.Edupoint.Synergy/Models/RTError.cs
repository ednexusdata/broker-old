using System.Xml;
using System.Xml.Serialization;

namespace OregonNexus.Broker.Connector.Edupoint.Synergy.Models;

[XmlRoot("RT_ERROR")]
public class RTError
{
    [XmlAttribute("ERROR_MESSAGE")]
    public string? ErrorMessage { get; set; }
}