using Microsoft.Extensions.DependencyInjection;

namespace OregonNexus.Broker.Connector;

public interface IConnectorServiceCollection
{
    static abstract IServiceCollection AddDependencies(IServiceCollection services);

}