using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using OregonNexus.Broker.Connector;
using OregonNexus.Broker.Web.Models;

namespace OregonNexus.Broker.Web.ViewComponents;

public class SettingsSidebarViewComponent : ViewComponent
{
    private readonly ConnectorLoader _connectorLoader;

    public SettingsSidebarViewComponent(ConnectorLoader connectorLoader)
    {
        _connectorLoader = connectorLoader;
    }

    public IViewComponentResult Invoke(string selectedView = "")
    {
        var viewModel = new SettingsSidebarViewModel();
        
        var connectors = _connectorLoader.Connectors;

        foreach(var connector in connectors)
        {   
            viewModel.Connectors.Add(new SettingsSidebarViewModel.ConnectorSidebarViewModel() {
                DisplayName = ((DisplayNameAttribute)connector.GetCustomAttributes(false).Where(x => x.GetType() == typeof(DisplayNameAttribute)).FirstOrDefault()!).DisplayName,
                ConnectorTypeName = connector.FullName!,
                Selected = (selectedView == connector.FullName!) ? true : false
            });
        }

        return View(viewModel);
    }

}