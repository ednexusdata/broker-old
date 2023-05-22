
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OregonNexus.Broker.Domain;
using OregonNexus.Broker.Domain.Specifications;
using OregonNexus.Broker.SharedKernel;
using OregonNexus.Broker.Web.Helpers;
using OregonNexus.Broker.Web.Models;

namespace OregonNexus.Broker.Web.Controllers;

[Authorize]
public class UserRolesController : Controller
{
    private readonly IRepository<UserRole> _userRoleRepo;

    private readonly IRepository<User> _userRepo;

    private readonly EducationOrganizationHelper _edOrgHelper;
    
    public UserRolesController(IRepository<UserRole> userRoleRepo, IRepository<User> userRepo, EducationOrganizationHelper edOrgHelper)
    {
        _userRoleRepo = userRoleRepo;
        _userRepo = userRepo;
        _edOrgHelper = edOrgHelper;
    }
    
    public async Task<IActionResult> Index(Guid? Id)
    {
        var user = await _userRepo.GetByIdAsync(Id);

        if (user is null) { return NotFound(); }

        var userRoleSpec = new UserRolesByUserSpec(user.Id);
        var userRoles = await _userRoleRepo.ListAsync(userRoleSpec);

        var userRolesViewModel = new UserRolesViewModel() {
            UserId = user.Id,
            User = user,
            UserRoles = userRoles,
            EducationOrganizations = await _edOrgHelper.GetOrganizationsSelectList()
        };

        return View(userRolesViewModel);
    }

    [HttpPost]
    public IActionResult Create(Guid UserId, UserRolesViewModel model)
    {
        return NotFound();
    }

    public IActionResult Delete(Guid? Id)
    {
        return NotFound();
    }
}