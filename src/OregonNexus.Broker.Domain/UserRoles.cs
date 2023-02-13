// Copyright: 2023 Education Nexus Oregon
// Author: Makoa Jacobsen, makoa@makoajacobsen.com

namespace OregonNexus.Broker.Domain;

public class UserRole : BaseEntity
{
    public EducationOrganization? Education { get; set; }
    public Guid? EducationOrganizationId { get; set; }
    public User? User { get; set; }
    public Guid? UserId { get; set; }
    public Role Role { get; set; }
}

public enum Role
{
    Registrar
}