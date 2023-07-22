using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using OregonNexus.Broker.Data;
using OregonNexus.Broker.Domain;
using OregonNexus.Broker.Domain.Specifications;
using OregonNexus.Broker.SharedKernel;
using System.Security.Claims;

namespace OregonNexus.Broker.Web;

public class BrokerClaimsTransformation : IClaimsTransformation
{
    private readonly IRepository<User> _userRepo;
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly ILogger<BrokerClaimsTransformation> _logger;

    private User _user;

    public BrokerClaimsTransformation(ILogger<BrokerClaimsTransformation> logger, UserManager<IdentityUser<Guid>> userManager, IRepository<User> userRepo)
    {
        _logger = logger;
        _userManager = userManager;
        _userRepo = userRepo;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // Start claims
        List<Claim> claims = new List<Claim>();

        // Get user-specific settings
        var claimType = "SuperAdmin";
        if (!principal.HasClaim(claim => claim.Type == claimType))
        {
            if ((await GetCurrentUser(principal)).IsSuperAdmin == true)
            {
                claims.Add(new Claim(claimType, "true"));
            }
        }

        claimType = "AllEducationOrganizations";
        if (!principal.HasClaim(claim => claim.Type == claimType))
        {
            if ((await GetCurrentUser(principal)).AllEducationOrganizations != PermissionType.None)
            {
                claims.Add(new Claim(claimType, (await GetCurrentUser(principal)).AllEducationOrganizations.ToString()));
            }
        }
        
        // Get userroles
        claimType = "TransferRecords";
        if (!principal.HasClaim(claim => claim.Type == claimType))
        {
            if ((await GetCurrentUser(principal)).UserRoles.Count > 0 || (await GetCurrentUser(principal)).AllEducationOrganizations != PermissionType.None)
            {
                claims.Add(new Claim(claimType, "true"));
            }
        }

        if (claims.Count > 0)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaims(claims);

            principal.AddIdentity(claimsIdentity);
        }
        
        return principal;
    }

    private async Task<User> GetCurrentUser(ClaimsPrincipal principal)
    {
        if (_user is null)
        {
            _logger.LogInformation("Current user not loaded for claims processing. Loading user.");
            // Get logged in user
            var email = principal.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").FirstOrDefault()!.Value!;
            var userIdentity = await _userManager.FindByEmailAsync(email);

            _user = await _userRepo.GetBySpecAsync(new ReadOnlyUserSpec(userIdentity.Id));
        }
        else
        {
            _logger.LogInformation("Current user loaded.");
        }
        return _user;
    }
}