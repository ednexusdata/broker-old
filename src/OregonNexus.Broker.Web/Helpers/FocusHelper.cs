using Microsoft.AspNetCore.Mvc.Rendering;
using OregonNexus.Broker.Domain;
using OregonNexus.Broker.Domain.Specifications;
using OregonNexus.Broker.SharedKernel;

namespace OregonNexus.Broker.Web.Helpers;

public class FocusHelper
{
    private readonly IRepository<EducationOrganization> _edOrgRepo;
    private readonly IRepository<UserRole> _userRoleRepo;
    private readonly IRepository<User> _userRepo;
    private readonly ISession _session;

    public FocusHelper(IRepository<EducationOrganization> edOrgRepo, IRepository<UserRole> userRoleRepo, IRepository<User> userRepo, IHttpContextAccessor httpContextAccessor)
    {
        _edOrgRepo = edOrgRepo;
        _userRepo = userRepo;
        _userRoleRepo = userRoleRepo;
        _session = httpContextAccessor.HttpContext?.Session;
    }

    public async Task<IEnumerable<SelectListItem>> GetFocusableEducationOrganizationsSelectList()
    {
        var selectListItems = new List<SelectListItem>();

        // Get currently logged in user
        var currentUser = _session.GetObjectFromJson<User>("User.Current");
        var allEdOrgs = currentUser?.AllEducationOrganizations;

        if (currentUser?.Id == null) return selectListItems;

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
        }
        else
        {
            var userRoleSpec = new UserRolesByUserSpec(currentUser.Id);
            var userRoles = await _userRoleRepo.ListAsync(userRoleSpec);

            foreach(var userRole in userRoles)
            {
                selectListItems.Add(new SelectListItem() {
                    Text = $"{userRole.EducationOrganization.ParentOrganization?.Name} / {userRole.EducationOrganization.Name}",
                    Value = userRole.Id.ToString(),
                    Selected = (_session.GetString("Focus.EducationOrganization.Current") == userRole.EducationOrganizationId.ToString())
                });
            }
        }

        selectListItems = selectListItems.OrderBy(x => x.Text).ToList();

        if (_session.GetString("Focus.EducationOrganization.Current") == null)
        {
            if (selectListItems.FirstOrDefault() is not null)
            {
                _session.SetString("Focus.EducationOrganization.Current", selectListItems.FirstOrDefault()!.Value);
            }
            
        }

        return selectListItems;
    }

    public Guid? CurrentEdOrgFocus()
    {
        var currentEdOrgFocus = _session.GetString("Focus.EducationOrganization.Current");
        if (currentEdOrgFocus != "ALL")
        {
            return Guid.Parse(currentEdOrgFocus);
        }
        return null;
    }

    public async Task<Guid?> CurrentDistrictEdOrgFocus()
    {
        var currentEdOrgFocus = CurrentEdOrgFocus();

        // Check if district
        if (currentEdOrgFocus.HasValue)
        {
            var edOrg = await _edOrgRepo.GetByIdAsync(currentEdOrgFocus.Value);
            if (edOrg is not null && edOrg.EducationOrganizationType == EducationOrganizationType.District)
            {
                return currentEdOrgFocus;
            }
        }
        return null;
    }

    public bool IsEdOrgAllFocus()
    {
        var currentEdOrgFocus = _session.GetString("Focus.EducationOrganization.Current");
        if (currentEdOrgFocus == "ALL")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}