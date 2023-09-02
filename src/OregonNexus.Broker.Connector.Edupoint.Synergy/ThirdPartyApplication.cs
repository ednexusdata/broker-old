using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using OregonNexus.Broker.Connector.Edupoint.Synergy.Models;
using System.Web;
using OregonNexus.Broker.Connector.Authentication;
using Microsoft.AspNetCore.Http;
using OregonNexus.Broker.Connector;

namespace OregonNexus.Broker.Connector.Edupoint.Synergy.Authentication;

/*
 <UserData UID="96870498-10C3-4D16-8B11-27BA3651C2CE" Login="admin" FullName="Admin User" Email="email@edupoint.com" />

 <RT_ERROR ERROR_MESSAGE="Unable to authenticate Synergy user" />
*/

public class ThirdPartyApplication : IAuthenticationProvider
{
    private const string PROVIDER_URL_NAME = "Edupoint-Synergy";

    private const string PROVIDER_DISPLAY_NAME = "Synergy";

    public string? BaseURL { get; set; }

    public static string ProviderUrlName()
    {
        return PROVIDER_URL_NAME;
    }

    public static string ProviderDisplayName()
    {
        return PROVIDER_DISPLAY_NAME;
    }

    public async Task<AuthenticatedUser> AuthenticateAsync(HttpRequest request)
    {
        
        return new AuthenticatedUser()
        {
            
        };
    }

    [System.Serializable]
    public class AuthenticationException : System.Exception
    {
        public RTError Response { get; set; }
        public AuthenticationException(string message, RTError response) : base(message)
        {
            this.Response = response;
        }
    }
}