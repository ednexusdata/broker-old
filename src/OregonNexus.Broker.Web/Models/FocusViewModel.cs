using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OregonNexus.Broker.Domain;

namespace OregonNexus.Broker.Web.Models;

public class FocusViewModel
{
    public string ReturnUrl { get; set; } = default!;
    public EducationOrganization? FocusEducationOrganization { get; set; }
}