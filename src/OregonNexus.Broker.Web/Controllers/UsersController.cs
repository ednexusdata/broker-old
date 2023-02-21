using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

    public UsersController(IRepository<User> repo, BrokerDbContext db)
    {
        _repo = repo;
        _db = db;
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

    public class CombinedUser
    {
        public IdentityUser<Guid>? IdentityUser { get; set; }
        public User? ApplicationUser { get; set; }
    }
}