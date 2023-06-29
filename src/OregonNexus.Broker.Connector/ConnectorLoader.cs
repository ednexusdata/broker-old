using System.Reflection;
using Microsoft.Extensions.Logging;

namespace OregonNexus.Broker.Connector;

public class ConnectorLoader
{
    //private readonly ILogger<ConnectorLoader> _logger;

    public List<Type> Connectors { get; private set; } = new List<Type>();

    public bool IsLoaded { get; set; } = false;

    public ConnectorLoader()
    {
        if (IsLoaded == false)
        {
            LoadConnectorAssemblies();
        }
    }

    public void LoadConnectorAssemblies()
    {
        IsLoaded = true;
        
        var connectorAssemblyPaths = Directory.GetFiles($"{System.AppDomain.CurrentDomain.BaseDirectory}connectors");
        if (connectorAssemblyPaths.Length == 0)
        {
            //_logger.LogInformation($"No connectors loaded from paths: {connectorAssemblyPaths}");
            return;
        }

        foreach(var assemblyPath in connectorAssemblyPaths)
        {
            if (String.IsNullOrEmpty(assemblyPath)) return;

            var fileInfo = new FileInfo(assemblyPath);
            if (fileInfo.Extension == ".dll")
            {
                Assembly.LoadFile(assemblyPath);
            }
        }

        var types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => p.GetInterface(nameof(IConnector)) is not null);

        foreach(var type in types)
        {
            if (type.GetInterface(nameof(IConnector)) != null)
            {
                Connectors.Add(type);
            }
        }
        
    }
}