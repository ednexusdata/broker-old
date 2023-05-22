using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using OregonNexus.Broker.Domain;

namespace OregonNexus.Broker.Web.Models;

public class UserRoleViewModel
{
    public Guid? UserId { get; set; }
    public Guid? EducationOrganizationId { get; set; }
    public Role? Role { get; set; }
}