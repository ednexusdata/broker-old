
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OregonNexus.Broker.SharedKernel;
using OregonNexus.Broker.Domain;

namespace OregonNexus.Broker.Web.Controllers;

public class EducationOrganizationsController : Controller
{
    private IRepository<EducationOrganization> _repo { get; set; }
    
    public EducationOrganizationsController(IRepository<EducationOrganization> repo)
    {
        _repo = repo;
    }

    public async Task<IActionResult> Index()
    {
        var data = await _repo.ListAsync();

        return View(data);
    }
}