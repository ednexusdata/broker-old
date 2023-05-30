using Microsoft.AspNetCore.Mvc.Rendering;
using OregonNexus.Broker.Domain;
using OregonNexus.Broker.Domain.Specifications;
using OregonNexus.Broker.SharedKernel;

namespace OregonNexus.Broker.Web.Helpers;

public class FocusHelper
{
    private readonly IRepository<EducationOrganization> _edOrgRepo;
    private readonly IRepository<User> _userRepo;
    private readonly ISession _session;

    public FocusHelper(IRepository<EducationOrganization> edOrgRepo, IRepository<User> userRepo, IHttpContextAccessor httpContextAccessor)
    {
        _edOrgRepo = edOrgRepo;
        _userRepo = userRepo;
        _session = httpContextAccessor.HttpContext?.Session;
    }

    public async Task<IEnumerable<SelectListItem>> GetFocusableEducationOrganizationsSelectList()
    {
        var selectListItems = new List<SelectListItem>();

        // Get currently logged in user
        var currentUser = _session.GetObjectFromJson<User>("User.Current");
        var allEdOrgs = currentUser?.AllEducationOrganizations;

        if (allEdOrgs == PermissionType.Read || allEdOrgs == PermissionType.Write)
        { 
            selectListItems.Add(new SelectListItem() {
                    Text = "All",
                    Value = "ALL",
                    Selected = (_session.GetString("Focus.EducationOrganization.Current") == "ALL")
                });
            
            var organizations = await _edOrgRepo.ListAsync();
            organizations = organizations.OrderBy(x => x.ParentOrganization?.Name).ThenBy(x => x.Name).ToList();

            foreach(var organization in organizations)
            {
                selectListItems.Add(new SelectListItem() {
                    Text = (organization.EducationOrganizationType == EducationOrganizationType.District) 
                        ? organization.Name 
                        : $"{organization.ParentOrganization?.Name} / {organization.Name}",
                    Value = organization.Id.ToString(),
                    Selected = (_session.GetString("Focus.EducationOrganization.Current") == organization.Id.ToString())
                });
            }
            
            selectListItems = selectListItems.OrderBy(x => x.Text).ToList();
        }

        if (_session.GetString("Focus.EducationOrganization.Current") == null)
        {
            if (selectListItems.FirstOrDefault() is not null)
            {
                _session.SetString("Focus.EducationOrganization.Current", selectListItems.FirstOrDefault()!.Value);
            }
            
        }

        return selectListItems;
    }
}