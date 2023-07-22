using System.Dynamic;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using OregonNexus.Broker.Connector;
using OregonNexus.Broker.Data;
using OregonNexus.Broker.Domain;
using OregonNexus.Broker.SharedKernel;
using OregonNexus.Broker.Web.Helpers;
using OregonNexus.Broker.Web.Models;

namespace OregonNexus.Broker.Web.Controllers;

//[Authorize(Policy = "SuperAdmin")]
public class SettingsController : Controller
{
    private readonly ConnectorLoader _connectorLoader;
    
    private readonly IServiceProvider _serviceProvider;

    public SettingsController(ConnectorLoader connectorLoader, IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(connectorLoader);
        
        _connectorLoader = connectorLoader;
        _serviceProvider = serviceProvider;
    }

    public IActionResult Index()
    {
        var connectors = _connectorLoader.Connectors;

        dynamic mymodel = new ExpandoObject();
        mymodel.connectors = connectors;
        mymodel.models = new List<Object>();

        foreach(var connector in connectors)
        {
            var configurationModels = connector.Assembly.GetTypes().Where(t => typeof(OregonNexus.Broker.Connector.Configuration.IConfiguration).IsAssignableFrom(t));
            
            Type first = configurationModels.FirstOrDefault();

            var iconfigModel = ActivatorUtilities.CreateInstance(_serviceProvider, first) as OregonNexus.Broker.Connector.Configuration.IConfiguration;
            
            var prop = iconfigModel.GetType().GetProperty("SynergyUrl");
            prop.SetValue(iconfigModel, "https://test.url");
            prop = iconfigModel.GetType().GetProperty("Username");
            prop.SetValue(iconfigModel, "TestUser");
            prop = iconfigModel.GetType().GetProperty("Password");
            prop.SetValue(iconfigModel, "TestPass");

            var html = ModelFormBuilderHelper.HtmlForModel(iconfigModel);

            var toSave = new { model = first, html = html };

            mymodel.models.Add(toSave);
        }

        return View(mymodel);
    }
}