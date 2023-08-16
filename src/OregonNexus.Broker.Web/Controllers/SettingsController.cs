using System.Dynamic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using OregonNexus.Broker.Connector;
using OregonNexus.Broker.Connector.Configuration;
using OregonNexus.Broker.Data;
using OregonNexus.Broker.Domain;
using OregonNexus.Broker.Domain.Specifications;
using OregonNexus.Broker.SharedKernel;
using OregonNexus.Broker.Web.Helpers;
using OregonNexus.Broker.Web.Models;

namespace OregonNexus.Broker.Web.Controllers;

[Authorize(Policy = "SuperAdmin")]
public class SettingsController : Controller
{
    private readonly ConnectorLoader _connectorLoader;
    private readonly ConfigurationSerializer _configurationSerializer;
    private readonly IServiceProvider _serviceProvider;
    private readonly IRepository<EducationOrganizationConnectorSettings> _repo;
    private readonly FocusHelper _focusHelper;

    private Guid? _focusedDistrictEdOrg { get; set; }

    public SettingsController(ConnectorLoader connectorLoader, IServiceProvider serviceProvider, IRepository<EducationOrganizationConnectorSettings> repo, FocusHelper focusHelper, ConfigurationSerializer configurationSerializer)
    {
        ArgumentNullException.ThrowIfNull(connectorLoader);
        
        _configurationSerializer = configurationSerializer;
        _connectorLoader = connectorLoader;
        _serviceProvider = serviceProvider;
        _repo = repo;
        _focusHelper = focusHelper;
    }

    public async Task<IActionResult> Index()
    {
        if (await FocusedToDistrict() is not null) return View();
        
        var connectors = _connectorLoader.Connectors;

        var settingsViewModel = new SettingsViewModel() {
            ConnectorTypes = connectors
        };

        return View(settingsViewModel);
    }

    [HttpGet("/Settings/Configuration/{assemblyQualifiedName}")]
    public async Task<IActionResult> Configuration(string assemblyQualifiedName)
    {
        if (await FocusedToDistrict() is not null) return await FocusedToDistrict();
        
        if (assemblyQualifiedName == "OregonNexus.Broker.Connector.Edupoint.Synergy.Configuration.Connection")
            assemblyQualifiedName = "OregonNexus.Broker.Connector.Edupoint.Synergy.Configuration.Connection, OregonNexus.Broker.Connector.Edupoint.Synergy, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        
        if (assemblyQualifiedName == "OregonNexus.Broker.Connector.EdFiAlliance.EdFi.Configuration.Connection")
            assemblyQualifiedName = "OregonNexus.Broker.Connector.EdFiAlliance.EdFi.Configuration.Connection, OregonNexus.Broker.Connector.EdFiAlliance.EdFi, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        
        // Get Connector Config Type
        Type connectorConfigType = Type.GetType(assemblyQualifiedName!, true)!;

        var iconfigModel = await _configurationSerializer.DeseralizeAsync(connectorConfigType, _focusedDistrictEdOrg.Value);
        
        return View(new { form = ModelFormBuilderHelper.HtmlForModel(iconfigModel) });
    }

    [HttpPost]
    public async Task<IActionResult> Update(IFormCollection collection)
    {
        if (await FocusedToDistrict() is not null) return await FocusedToDistrict();

        var assemblyQualifiedName = collection["ConnectorConfigurationType"];

        // Get Connector Config Type
        Type connectorConfigType = Type.GetType(assemblyQualifiedName!, true)!;

        var iconfigModel = await _configurationSerializer.DeseralizeAsync(connectorConfigType, _focusedDistrictEdOrg.Value);

        // Loop through properties and set from form
        foreach(var prop in iconfigModel.GetType().GetProperties())
        {
            prop.SetValue(iconfigModel, collection[prop.Name].ToString());
        }

        await _configurationSerializer.SerializeAndSaveAsync(iconfigModel, _focusedDistrictEdOrg.Value);

        TempData["Success"] = $"Updated Settings.";

        return RedirectToAction("Configuration", new { assemblyQualifiedName = connectorConfigType.FullName });
    }

    private async Task<IActionResult?> FocusedToDistrict()
    {
        if (_focusedDistrictEdOrg == null)
        {
            _focusedDistrictEdOrg = await _focusHelper.CurrentDistrictEdOrgFocus();
        }
        
        if (!_focusedDistrictEdOrg.HasValue)
        {
            TempData["Error"] = $"Must be focused to a district.";
            return RedirectToAction("Index");
        }

        return null;
    }
}