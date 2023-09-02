// Copyright: 2023 Education Nexus Oregon
// Author: Makoa Jacobsen, makoa@makoajacobsen.com

using Microsoft.AspNetCore.Mvc;
using OregonNexus.Broker.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using OregonNexus.Broker.Domain;
using System.Text.Json;
using OregonNexus.Broker.SharedKernel;
using System.Security.Claims;

namespace OregonNexus.Broker.Web.Controllers;

[AllowAnonymous]
public class LoginController : Controller
{
    private readonly ILogger<LoginController> _logger;
    //private readonly OregonNexus.Broker.Connector.Edupoint.Synergy.Authentication.ThirdPartyApplication _auth;
    private readonly ISession? _session;
    //private readonly AuthenticationProvidersLocator? _authProvidersLocator;
    public readonly BrokerDbContext _db;
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly SignInManager<IdentityUser<Guid>> _signInManager;
    private readonly IRepository<User> _userRepo;

    public LoginController(ILogger<LoginController> logger, UserManager<IdentityUser<Guid>> userManager,
        IHttpContextAccessor httpContextAccessor, BrokerDbContext BrokerDbContext, SignInManager<IdentityUser<Guid>> signinManager,
        IRepository<User> userRepo)
    {
        _logger = logger;
        _session = httpContextAccessor.HttpContext?.Session;
        _db = BrokerDbContext;
        _userManager = userManager;
        _signInManager = signinManager;
        _userRepo = userRepo;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var externalLogins = await _signInManager.GetExternalAuthenticationSchemesAsync();
        return View(externalLogins);
    }

    [HttpGet]
    public async Task<IActionResult> CreateFirstUser()
    {
        var identityUser = new IdentityUser<Guid> { UserName = "mjacobsen@clackesd.k12.or.us", Email = "mjacobsen@clackesd.k12.or.us" }; 
        var result = await _userManager.CreateAsync(identityUser);

        var user = new User()
        {
            Id = identityUser.Id,
            FirstName = "Makoa",
            LastName = "Jacobsen",
            IsSuperAdmin = true,
            CreatedAt = DateTime.UtcNow
        };
        _db.Add(user);
        await _db.SaveChangesAsync();
        
        return RedirectToAction("index");
    }

    [HttpPost]
    [Route("login/externallogin")]
    public IActionResult ExternalLogin(string provider, string? returnUrl)
    {
        // Request a redirect to the external login provider.
        var redirectUrl = Url.Action("ExternalLoginCallback", values: new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    [HttpGet]
    [Route("login/externallogin")]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl, string? remoteError)
    {
        returnUrl = returnUrl ?? Url.Content("~/");
        if (remoteError != null)
        {
            //ErrorMessage = $"Error from external provider: {remoteError}";
            return RedirectToAction("Login", new { ReturnUrl = returnUrl });
        }
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            //ErrorMessage = "Error loading external login information.";
            return RedirectToAction("Login", new { ReturnUrl = returnUrl });
        }

        // Sign in the user with this external login provider if the user already has a login.
        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (result.Succeeded)
        {
            var email = info.Principal.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").FirstOrDefault()!.Value!;
            var user = await _userManager.FindByEmailAsync(email);

            var currentUser = await _userRepo.GetByIdAsync(user?.Id);
            _session?.SetObjectAsJson("User.Current", currentUser);
            
            _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info!.Principal.Identity?.Name, info.LoginProvider);
            return LocalRedirect(returnUrl);
        }
        if (result.IsLockedOut)
        {
            return RedirectToAction("Lockout");
        }
        else
        {
            // Get user
            var email = info.Principal.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").FirstOrDefault()!.Value!;
            var user = await _userManager.FindByEmailAsync(email);

            var currentUser = await _userRepo.GetByIdAsync(user?.Id);
            _session?.SetObjectAsJson("User.Current", currentUser);

            if (user is null)
            {
                _logger.LogInformation("{Email} not found in database.", email);
                return RedirectToAction("Login");
            }
            
            var loginResult = await _userManager.AddLoginAsync(user, info);
            if (loginResult.Succeeded)
            {
                _logger.LogInformation("Added {Name} logged in with {LoginProvider} provider.", info!.Principal.Identity?.Name, info.LoginProvider);
            }
            result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info!.Principal.Identity?.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            return RedirectToAction("Login");
        }
    }

    [AllowAnonymous]
    [Route("login/logout")]
    public async Task<IActionResult> Logout()
    {
         await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out.");
        return RedirectToAction("Index", "Home");
    }

}

public class LogInViewModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }

        public IList<AuthenticationScheme>? ExternalLogins { get; set; }
    }