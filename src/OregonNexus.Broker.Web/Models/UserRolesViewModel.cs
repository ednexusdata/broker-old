using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using OregonNexus.Broker.Domain;

namespace OregonNexus.Broker.Web.Models;

public class UserRolesViewModel
{
    public Guid? UserId { get; set; }

    public User? User { get; set; }
}