using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using OregonNexus.Broker.Connector;
using OregonNexus.Broker.Data;
using OregonNexus.Broker.Domain;
using OregonNexus.Broker.SharedKernel;
using OregonNexus.Broker.Web.Models;

namespace OregonNexus.Broker.Web.Controllers;

[Authorize(Policy = "SuperAdmin")]
public class SettingsController : Controller
{
    private readonly ConnectorLoader _connectorLoader;

    public SettingsController(ConnectorLoader connectorLoader)
    {
        _connectorLoader = connectorLoader;
    }

    public IActionResult Index()
    {
        var connectors = _connectorLoader.Connectors;

        return View(connectors);
    }
}