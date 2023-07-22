
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OregonNexus.Broker.SharedKernel;
using OregonNexus.Broker.Domain;
using OregonNexus.Broker.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using OregonNexus.Broker.Domain.Specifications;
using OregonNexus.Broker.Web.Helpers;

namespace OregonNexus.Broker.Web.Controllers;

[Authorize(Policy = "SuperAdmin")]
public class EducationOrganizationsController : Controller
{
    private readonly IRepository<EducationOrganization> _repo;
    private readonly EducationOrganizationHelper _edOrgHelper;
    
    public EducationOrganizationsController(IRepository<EducationOrganization> repo, EducationOrganizationHelper edOrgHelper)
    {
        _repo = repo;
        _edOrgHelper = edOrgHelper;
    }

    public async Task<IActionResult> Index()
    {
        var data = await _repo.ListAsync();
        data = data.OrderBy(x => x.ParentOrganization?.Name).ThenBy(x => x.Name).ToList();

        var viewData = new List<EducationOrganization>();
        while(data.Count > 0)
        {
            var eo = data.FirstOrDefault();
            if (eo is not null)
            {
                // Add district
                viewData.Add(eo);
                data.Remove(eo);
                // add the schools
                var schools = data.Where(x => x.ParentOrganization == eo).OrderBy(x => x.Name).ToList();
                foreach(var seo in schools)
                {
                    viewData.Add(seo);
                    data.Remove(seo);
                }
            }
        }

        return View(viewData);
    }

    public async Task<IActionResult> Add()
    {
        var organizationViewModel = new EducationOrganizationViewModel();

        // Get Organizations
        organizationViewModel.Organizations = await _edOrgHelper.GetDistrictsOrganizationsSelectList();
        
        return View(organizationViewModel);
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Create(EducationOrganizationViewModel data)
    {
        if (!ModelState.IsValid) { TempData["Error"] = "Organization not created."; return View("Add"); }

        var organization = new EducationOrganization()
        {
            Id = Guid.NewGuid(),
            ParentOrganizationId = (data.EducationOrganizationType == EducationOrganizationType.School) ? data.ParentOrganizationId : null,
            Name = data.Name,
            Number = data.Number,
            EducationOrganizationType = data.EducationOrganizationType
        };

        await _repo.AddAsync(organization);

        TempData["Success"] = $"Created organization {organization.Name} ({organization.Id}).";

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(Guid Id)
    {
        var organization = await _repo.GetByIdAsync(Id);

        var organizationViewModel = new EducationOrganizationViewModel();

        if (organization is not null)
        {
            organizationViewModel = new EducationOrganizationViewModel()
            {
                EducationOrganizationId = organization.Id,
                ParentOrganizationId = organization.ParentOrganizationId,
                Name = organization.Name!,
                EducationOrganizationType = organization.EducationOrganizationType,
                Number = organization.Number!
            };
        }

        // Get Organizations
        organizationViewModel.Organizations = await _edOrgHelper.GetDistrictsOrganizationsSelectList(Id);

        return View(organizationViewModel);
    }

    [ValidateAntiForgeryToken]
    [HttpPatch]
    public async Task<IActionResult> Update(EducationOrganizationViewModel data)
    {
        if (!data.EducationOrganizationId.HasValue) { throw new ArgumentNullException("EducationOrganizationId required."); }
        
        // Get existing organization
        var organization = await _repo.GetByIdAsync(data.EducationOrganizationId.Value);

        if (organization is null) { throw new ArgumentException("Not a valid organization."); }

        if (!ModelState.IsValid) { TempData["Error"] = "Organization not updated."; return View("Edit"); }

        // Prepare organization object
        organization.Name = data.Name;
        if (data.EducationOrganizationType == EducationOrganizationType.School)
        {
            organization.ParentOrganizationId = data.ParentOrganizationId;
        }
        else
        {
            organization.ParentOrganizationId = null;
        }
        organization.Number = data.Number;
        organization.EducationOrganizationType = data.EducationOrganizationType;

        await _repo.UpdateAsync(organization);

        TempData["Success"] = $"Updated organization {organization.Name} ({organization.Id}).";

        return RedirectToAction("Edit", new { Id = organization.Id });
    }

    [ValidateAntiForgeryToken]
    [HttpDelete]
    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id is null) { throw new ArgumentNullException("Missing id for organization to delete."); }
        
        var organization = await _repo.GetByIdAsync(id.Value);

        if (organization is null) { throw new ArgumentException("Not a valid organization."); }

        await _repo.DeleteAsync(organization);

        TempData["Success"] = $"Deleted organization {organization.Name} ({organization.Id}).";

        return RedirectToAction("Index");
    }

}