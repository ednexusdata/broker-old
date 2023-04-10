using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using OregonNexus.Broker.Data;
using OregonNexus.Broker.Domain;
using OregonNexus.Broker.SharedKernel;

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
        var combinedUsers = new List<CombinedUser>();
        
        var identityUsers = await _db.Users.OrderBy(x => x.NormalizedEmail).ToListAsync();
        var users = await _repo.ListAsync();

        foreach(var identityUser in identityUsers)
        {
            var combinedUser = new CombinedUser();
            combinedUser.IdentityUser = identityUser;
            combinedUser.ApplicationUser = users.Where(x => x.Id == identityUser.Id).FirstOrDefault();

            combinedUsers.Add(combinedUser);
        }

        combinedUsers.OrderBy(x => x.ApplicationUser?.LastName).OrderBy(x => x.ApplicationUser?.FirstName);

        return View(combinedUsers);
    }

    public IActionResult Add()
    {
        return View();
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Create(CombinedUser data)
    {
        var identityUser = new IdentityUser<Guid> { UserName = data.IdentityUser.Email, Email = data.IdentityUser.Email }; 
        var result = await _userManager.CreateAsync(identityUser);

        var user = new User()
        {
            Id = identityUser.Id,
            FirstName = data.ApplicationUser.FirstName,
            LastName = data.ApplicationUser.LastName,
            IsSuperAdmin = data.ApplicationUser.IsSuperAdmin
        };

        await _repo.AddAsync(user);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(Guid Id)
    {
        var combinedUser = new CombinedUser();

        combinedUser.IdentityUser = await _db.Users.Where(x => x.Id == Id).FirstOrDefaultAsync();
        combinedUser.ApplicationUser = await _repo.GetByIdAsync(Id);

        return View(combinedUser);
    }

    [ValidateAntiForgeryToken]
    [HttpPatch]
    public async Task<IActionResult> Update(CombinedUser data)
    {
        // Get existing user
        var user = await _userManager.FindByIdAsync(data.IdentityUser.Id.ToString());

        if (data.IdentityUser.Email != user.Email)
        {
            // Update email
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, data.IdentityUser.Email);
            var result = await _userManager.ChangeEmailAsync(user, data.IdentityUser.Email, token);

            // Update user
            var userUpdateResult = await _userManager.SetUserNameAsync(user, data.IdentityUser.Email.ToLower());
        }

        // Prepare user object
        var appUser = data.ApplicationUser;
        appUser.Id = data.IdentityUser.Id;

        await _repo.UpdateAsync(appUser);

        return RedirectToAction("Edit", new { Id = data.IdentityUser.Id });
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid? id)
    {
        var identityUser = await _userManager.FindByIdAsync(id.ToString());
        var applicationUser = await _repo.GetByIdAsync(id);

        await _repo.DeleteAsync(applicationUser);
        await _userManager.DeleteAsync(identityUser);

        return RedirectToAction("Index");
    }

    public class CombinedUser
    {
        public IdentityUser<Guid>? IdentityUser { get; set; }
        public User? ApplicationUser { get; set; }
    }
}