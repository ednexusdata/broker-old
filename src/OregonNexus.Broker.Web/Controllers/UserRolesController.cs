
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OregonNexus.Broker.Domain;
using OregonNexus.Broker.SharedKernel;
using OregonNexus.Broker.Web.Models;

namespace OregonNexus.Broker.Web.Controllers;

[Authorize]
public class UserRolesController : Controller
{
    private readonly IRepository<UserRole> _userRoleRepo;

    private readonly IRepository<User> _userRepo;
    
    public UserRolesController(IRepository<UserRole> userRoleRepo, IRepository<User> userRepo)
    {
        _userRoleRepo = userRoleRepo;
        _userRepo = userRepo;
    }
    
    public async Task<IActionResult> Index(Guid? Id)
    {
        var user = await _userRepo.GetByIdAsync(Id);

        var userRolesViewModel = new UserRolesViewModel() {
            User = user
        };

        return View(userRolesViewModel);
    }
}