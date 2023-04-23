using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using OregonNexus.Broker.Data;
using OregonNexus.Broker.Domain;
using OregonNexus.Broker.SharedKernel;
using OregonNexus.Broker.Web.Models;

namespace OregonNexus.Broker.Web.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly IRepository<User> _repo;
    private readonly BrokerDbContext _db;
    private readonly UserManager<IdentityUser<Guid>> _userManager;

    public UsersController(IRepository<User> repo, BrokerDbContext db, UserManager<IdentityUser<Guid>> userManager)
    {
        _repo = repo;
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var combinedUsers = new List<UserViewModel>();
        
        var identityUsers = await _db.Users.OrderBy(x => x.NormalizedEmail).ToListAsync();
        var users = await _repo.ListAsync();

        foreach(var identityUser in identityUsers)
        {
            var appUser = users.Where(x => x.Id == identityUser.Id).FirstOrDefault();

            var combinedUser = new UserViewModel()
            {
                UserId = identityUser.Id,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                IsSuperAdmin = appUser.IsSuperAdmin,
                Email = identityUser.Email
            };

            combinedUsers.Add(combinedUser);
        }

        combinedUsers.OrderBy(x => x.LastName).OrderBy(x => x.FirstName);

        return View(combinedUsers);
    }

    public IActionResult Add()
    {
        return View();
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Create(UserViewModel data)
    {
        if (!ModelState.IsValid) { return View("Add"); }
        
        var identityUser = new IdentityUser<Guid> { UserName = data.Email, Email = data.Email }; 
        var result = await _userManager.CreateAsync(identityUser);

        var user = new User()
        {
            Id = identityUser.Id,
            FirstName = data.FirstName,
            LastName = data.LastName,
            IsSuperAdmin = data.IsSuperAdmin
        };

        await _repo.AddAsync(user);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(Guid Id)
    {
        var identityUser = await _db.Users.Where(x => x.Id == Id).FirstOrDefaultAsync();
        var applicationUser = await _repo.GetByIdAsync(Id);

        var userViewModel = new UserViewModel();

        if (applicationUser is not null && identityUser is not null)
        {
            userViewModel = new UserViewModel()
            {
                UserId = applicationUser.Id,
                IdentityUser = identityUser,
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName,
                IsSuperAdmin = applicationUser.IsSuperAdmin,
                Email = identityUser.Email
            };
        }

        return View(userViewModel);
    }

    [ValidateAntiForgeryToken]
    [HttpPatch]
    public async Task<IActionResult> Update(UserViewModel data)
    {
        // Get existing user
        var user = await _userManager.FindByIdAsync(data.UserId.ToString());

        if (user is null) { throw new ArgumentException("Not a valid user."); }

        if (!ModelState.IsValid) { return View("Edit"); }

        if (data.Email != user.Email)
        {
            // Update email
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, data.Email);
            var result = await _userManager.ChangeEmailAsync(user, data.Email, token);

            // Update user
            var userUpdateResult = await _userManager.SetUserNameAsync(user, data.Email.ToLower());
        }

        // Prepare user object
        var appUser = new User()
        {
            Id = user.Id,
            FirstName = data.FirstName,
            LastName = data.LastName,
            IsSuperAdmin = data.IsSuperAdmin
        };

        await _repo.UpdateAsync(appUser);

        return RedirectToAction("Edit", new { Id = data.UserId });
    }

    [ValidateAntiForgeryToken]
    [HttpDelete]
    public async Task<IActionResult> Delete(Guid? id)
    {
        var identityUser = await _userManager.FindByIdAsync(id.ToString());
        var applicationUser = await _repo.GetByIdAsync(id);

        await _repo.DeleteAsync(applicationUser);
        await _userManager.DeleteAsync(identityUser);

        return RedirectToAction("Index");
    }
}