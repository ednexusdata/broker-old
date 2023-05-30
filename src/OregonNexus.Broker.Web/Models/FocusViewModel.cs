using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OregonNexus.Broker.Domain;

namespace OregonNexus.Broker.Web.Models;

public class FocusViewModel
{
    public string ReturnUrl { get; set; } = default!;
    
    // This is a string because it could be ALL or a Guid for an EducationOrganization.
    public String? FocusEducationOrganizationId { get; set; }
}