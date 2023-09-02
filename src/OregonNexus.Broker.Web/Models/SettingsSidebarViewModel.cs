using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using OregonNexus.Broker.Connector;
using OregonNexus.Broker.Domain;

namespace OregonNexus.Broker.Web.Models;

public class SettingsSidebarViewModel
{
    public List<ConnectorSidebarViewModel> Connectors { get; set; } = new List<ConnectorSidebarViewModel>();
    public List<PayloadSidebarViewModel> Payloads { get; set; } = new List<PayloadSidebarViewModel>();

    public class ConnectorSidebarViewModel
    {
        public string DisplayName = default!;

        public string ConnectorTypeName = default!;
        public bool Selected { get; set; } = false;
    }

    public class PayloadSidebarViewModel
    {
        public string DisplayName { get; set; } = default!;
        public string PayloadTypeName { get; set; } = default!;
        public bool Selected { get; set; } = false;
    }
}

