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
            if (connector is not null)
            {
                viewModel.Connectors.Add(new SettingsSidebarViewModel.ConnectorSidebarViewModel() {
                    DisplayName = ((DisplayNameAttribute)connector.GetCustomAttributes(false).Where(x => x.GetType() == typeof(DisplayNameAttribute)).FirstOrDefault()!).DisplayName,
                    ConnectorTypeName = connector.Assembly.GetName().Name!,
                    Selected = (selectedView == connector.Assembly.GetName().Name) ? true : false
                });
            }
            
        }

        var payloads = _connectorLoader.Payloads;

        foreach(var payload in payloads)
        {   
            if (payload is not null)
            {
                viewModel.Payloads.Add(new SettingsSidebarViewModel.PayloadSidebarViewModel() {
                    DisplayName = ((DisplayNameAttribute)payload.GetCustomAttributes(false).Where(x => x.GetType() == typeof(DisplayNameAttribute)).FirstOrDefault()!).DisplayName,
                    PayloadTypeName = payload.Name,
                    Selected = (selectedView == payload.Name) ? true : false
                });
            }
            
        }

        return View(viewModel);
    }

}