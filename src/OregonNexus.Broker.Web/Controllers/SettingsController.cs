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
        var currentEdOrgFocus = await _focusHelper.CurrentDistrictEdOrgFocus();
        if (!currentEdOrgFocus.HasValue)
        {
            TempData["Error"] = $"Must be focused to a district.";
            return View();
        }
        
        var connectors = _connectorLoader.Connectors;

        dynamic mymodel = new ExpandoObject();
        mymodel.connectors = connectors;
        mymodel.models = new List<Object>();

        foreach(var connector in connectors)
        {
            var configurationModels = connector.Assembly.GetTypes().Where(t => typeof(OregonNexus.Broker.Connector.Configuration.IConfiguration).IsAssignableFrom(t));
            
            Type first = configurationModels.FirstOrDefault();

            var iconfigModel = await _configurationSerializer.DeseralizeAsync(first, currentEdOrgFocus.Value);

            var html = ModelFormBuilderHelper.HtmlForModel(iconfigModel);

            var toSave = new { model = first, html = html };

            mymodel.models.Add(toSave);
        }

        return View(mymodel);
    }

    [HttpPost]
    public async Task<IActionResult> Update(IFormCollection collection)
    {
        var currentEdOrgFocus = await _focusHelper.CurrentDistrictEdOrgFocus();
        if (currentEdOrgFocus.HasValue)
        {
            TempData["Error"] = $"Must be focused to a district.";
            return View();
        }

        var assemblyQualifiedName = collection["ConnectorConfigurationType"];

        // Get Connector Config Type
        Type connectorConfigType = Type.GetType(assemblyQualifiedName!, true)!;

        var iconfigModel = await _configurationSerializer.DeseralizeAsync(connectorConfigType, currentEdOrgFocus.Value);

        // Loop through properties and set from form
        foreach(var prop in iconfigModel.GetType().GetProperties())
        {
            prop.SetValue(iconfigModel, collection[prop.Name].ToString());
        }

        await _configurationSerializer.SerializeAndSaveAsync(iconfigModel, currentEdOrgFocus.Value);

        TempData["Success"] = $"Updated Settings.";

        return RedirectToAction("Index");
    }
}