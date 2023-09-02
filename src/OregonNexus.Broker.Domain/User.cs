// Copyright: 2023 Education Nexus Oregon
// Author: Makoa Jacobsen, makoa@makoajacobsen.com

namespace OregonNexus.Broker.Domain;

public class User : BaseEntity, IAggregateRoot
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public bool IsSuperAdmin { get; set; } = false;
    public PermissionType AllEducationOrganizations { get; set; } = PermissionType.None;

    public List<UserRole>? UserRoles { get; set; }

    public string Name { 
        get {
            return $"{FirstName} {LastName}";
        }
    }
}
