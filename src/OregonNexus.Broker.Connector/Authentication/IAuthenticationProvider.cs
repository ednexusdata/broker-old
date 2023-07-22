using Microsoft.AspNetCore.Http;

namespace OregonNexus.Broker.Connector.Authentication;

public interface IAuthenticationProvider
{
    public static string ProviderUrlName() => throw new NotImplementedException("Must implement static method ProviderUrlName.");
    public static string ProviderDisplayName() => throw new NotImplementedException("Must implement static method ProviderDisplayName.");
    public Task<AuthenticatedUser> AuthenticateAsync(HttpRequest request);
}