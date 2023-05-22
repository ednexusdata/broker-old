using Microsoft.AspNetCore.Mvc.Rendering;
using OregonNexus.Broker.Domain;
using OregonNexus.Broker.Domain.Specifications;
using OregonNexus.Broker.SharedKernel;

namespace OregonNexus.Broker.Web.Helpers;

public class EducationOrganizationHelper
{
    private IRepository<EducationOrganization> _repo { get; set; }
    public EducationOrganizationHelper(IRepository<EducationOrganization> repo)
    {
        _repo = repo;
    }
    
    public async Task<IEnumerable<SelectListItem>> GetOrganizationsSelectList(Guid? focusedOrganizationId = null)
    {
        var selectListItems = new List<SelectListItem>();

        var organizations = await _repo.ListAsync(new OrganizationByTypeSpec(EducationOrganizationType.District));
        organizations = organizations.OrderBy(x => x.Name).ToList();

        foreach(var organization in organizations)
        {
            if (focusedOrganizationId is null || (focusedOrganizationId is not null && organization.Id != focusedOrganizationId))
            {
                selectListItems.Add(new SelectListItem() {
                    Text = organization.Name,
                    Value = organization.Id.ToString()
                });
            }
        }

        return selectListItems;
    }
}