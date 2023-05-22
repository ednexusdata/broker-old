using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using OregonNexus.Broker.Domain;

namespace OregonNexus.Broker.Web.Models;

public class UserRolesViewModel
{
    public Guid? UserId { get; set; }

    public User? User { get; set; }

    public List<UserRole>? UserRoles { get; set; }

    public IEnumerable<SelectListItem>? EducationOrganizations { get; set; }

    public UserRoleViewModel TemplateUserRoleViewModel => new UserRoleViewModel();

    public Guid? EducationOrganizationId { get; set; }
    public Role? Role { get; set; }
}