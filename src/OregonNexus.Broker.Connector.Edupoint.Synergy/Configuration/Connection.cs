using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using OregonNexus.Broker.Connector.Configuration;

namespace OregonNexus.Broker.Connector.Edupoint.Synergy.Configuration;

public class Connection : IConfiguration
{
    [DataType(DataType.Url)]
    [DisplayName("Synergy URL")]
    [Description("The URL of the Synergy instance to connect to.")]
    public string SynergyUrl { get; set; } = default!;

    [DataType(DataType.Text)]
    [Description("The username of the user to connect to the Synergy instance specified in Synergy URL.")]
    public string Username { get; set; } = default!;

    [DataType(DataType.Text)]
    [Description("The password of the user to connect to the Synergy instance specified in Synergy URL.")]
    public string Password { get; set; } = default!;
}