using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
//using OregonNexus.Broker.Connector.Authentication;
//using OregonNexus.Broker.Connector.Locators;

namespace OregonNexus.Broker.Connector.EdFiAlliance.EdFi;

public class ServiceCollection : IConnectorServiceCollection
{
    /*
    public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration config)
    {
        return services;
    }
    */

    public static IServiceCollection AddDependencies(IServiceCollection services)
    {
        return services;
    }
}