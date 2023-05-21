using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using OregonNexus.Broker.Domain;

namespace OregonNexus.Broker.Web.Models;

public class EducationOrganizationViewModel
{
    public Guid? EducationOrganizationId { get; set; }

    [Required]
    [Display(Name = "Name")]
    public string Name { get; set; } = default!;

    [Required]
    [Display(Name = "Number")]
    public string Number { get; set; } = default!;

    [Required]
    [Display(Name = "Type")]
    public EducationOrganizationType EducationOrganizationType { get; set; } = default!;

    [Display(Name = "District")]
    public Guid? ParentOrganizationId { get; set; }

    public IEnumerable<SelectListItem>? Organizations { get; set; }
}
