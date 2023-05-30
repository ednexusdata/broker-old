using Microsoft.AspNetCore.Mvc.Rendering;
using OregonNexus.Broker.Domain;
using OregonNexus.Broker.Domain.Specifications;
using OregonNexus.Broker.SharedKernel;

namespace OregonNexus.Broker.Web.Helpers;

public class FocusHelper
{
    private readonly IRepository<EducationOrganization> _edOrgRepo;
    private readonly IRepository<User> _userRepo;
    private readonly IHttpContextAccessor _httpContext;

    public FocusHelper(IRepository<EducationOrganization> edOrgRepo, IRepository<User> userRepo, IHttpContextAccessor httpContext)
    {
        _edOrgRepo = edOrgRepo;
        _userRepo = userRepo;
        _httpContext = httpContext;
    }

    public async Task<IEnumerable<SelectListItem>> GetFocusableEducationOrganizationsSelectList()
    {
        var selectListItems = new List<SelectListItem>();

        // Get currently logged in user
        var currentUser = _httpContext.HttpContext?.Session?.GetObjectFromJson<User>("User.Current");
        var allEdOrgs = currentUser?.AllEducationOrganizations;

        if (allEdOrgs == PermissionType.Read || allEdOrgs == PermissionType.Write)
        {
            var organizations = await _edOrgRepo.ListAsync();
            organizations = organizations.OrderBy(x => x.ParentOrganization?.Name).ThenBy(x => x.Name).ToList();

            foreach(var organization in organizations)
            {
                selectListItems.Add(new SelectListItem() {
                    Text = (organization.EducationOrganizationType == EducationOrganizationType.District) 
                        ? organization.Name 
                        : $"{organization.ParentOrganization?.Name} / {organization.Name}",
                    Value = organization.Id.ToString()
                });
            }
            
            selectListItems = selectListItems.OrderBy(x => x.Text).ToList();
        }
        return selectListItems;
    }
}