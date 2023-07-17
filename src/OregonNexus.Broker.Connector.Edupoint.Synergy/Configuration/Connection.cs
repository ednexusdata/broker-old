using System.ComponentModel.DataAnnotations;
using OregonNexus.Broker.Connector.Configuration;

namespace OregonNexus.Broker.Connector.Edupoint.Synergy.Configuration;

public class Connection : IConfiguration
{
    [DataType(DataType.Url)]
    public string SynergyUrl { get; set; } = default!;

    [DataType(DataType.Text)]
    public string Username { get; set; } = default!;

    [DataType(DataType.Password)]
    public string Password { get; set; } = default!;
}