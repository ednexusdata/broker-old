using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OregonNexus.Broker.Connector;

public class ConnectorLoader
{
    //private readonly ILogger<ConnectorLoader> _logger;

    public List<Type> Connectors { get; private set; } = new List<Type>();

    public Dictionary<string, Assembly> Assemblies { get; private set; } = new Dictionary<string, Assembly>();

    public Dictionary<string, string> ConnectorIndex { get; private set; } = new Dictionary<string, string>();
    public Dictionary<string, string> ConfigurationIndex { get; private set; } = new Dictionary<string, string>();

    public bool IsLoaded { get; set; } = false;

    public ConnectorLoader()
    {
        if (IsLoaded == false)
        {
            LoadConnectorAssemblies();
            LoadConfigurations();
        }
    }

    public List<Type>? GetConfigurations(Assembly assembly)
    {
        return assembly.GetTypes().Where(x => x.GetInterface(nameof(Configuration.IConfiguration)) is not null).ToList();
    }

    private void LoadConnectorAssemblies()
    {
        IsLoaded = true;
        
        var logger = LoggerFactory.Create(config =>
        {
            config.AddConsole();
        }).CreateLogger("ConnectorLoader");

        var connectorAssemblyPaths = Directory.GetFiles($"{System.AppDomain.CurrentDomain.BaseDirectory}connectors");
        if (connectorAssemblyPaths.Length == 0)
        {
            logger.LogInformation($"No connectors loaded from paths: {connectorAssemblyPaths}");
            return;
        }

        foreach(var assemblyPath in connectorAssemblyPaths)
        {
            if (String.IsNullOrEmpty(assemblyPath)) return;

            var fileInfo = new FileInfo(assemblyPath);
            if (fileInfo.Extension == ".dll")
            {
                Assembly.LoadFrom(assemblyPath);
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
                Assemblies.Add(type.Assembly.GetName().Name!, type.Assembly);
                ConnectorIndex.Add(type.FullName!, type.AssemblyQualifiedName!);
                
                logger.LogInformation($"Connector loaded: {type.FullName} from {type.AssemblyQualifiedName}");
            }
        }
        
    }

    private void LoadConfigurations()
    {
        foreach(var connector in Connectors)
        {
            var configurations = connector.Assembly.GetTypes().Where(p => p.GetInterface(nameof(Configuration.IConfiguration)) is not null);
            if (configurations.Count() > 0)
            {
                foreach(var config in configurations)
                {
                    ConfigurationIndex.Add(config.FullName!, connector.AssemblyQualifiedName!);
                }
            }
        }
    }
}