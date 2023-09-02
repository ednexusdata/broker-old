using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using OregonNexus.Broker.Connector;
using OregonNexus.Broker.Domain;

namespace OregonNexus.Broker.Web.Models;

public class SettingsViewModel
{
    public List<Type>? ConnectorTypes { get; set; }

    public List<dynamic>? Models { get; set; } = new List<dynamic>();
}