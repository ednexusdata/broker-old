// Copyright: 2023 Education Nexus Oregon
// Author: Makoa Jacobsen, makoa@makoajacobsen.com

using System.ComponentModel.DataAnnotations;

namespace OregonNexus.Broker.Domain;

public class UserRole : BaseEntity, IAggregateRoot
{
    public EducationOrganization? EducationOrganization { get; set; }
    public Guid? EducationOrganizationId { get; set; }
    public User? User { get; set; }
    public Guid? UserId { get; set; }
    public Role Role { get; set; }
}

public enum Role
{
    [Display(Name = "Incoming Processor")]
    IncomingProcessor,

    [Display(Name = "Outgoing Processor")]
    OutgoingProcessor,
    
    [Display(Name = "Processor")]
    Processor,

    [Display(Name = "Read-Only Processor")]
    ReadOnlyProcessor
}