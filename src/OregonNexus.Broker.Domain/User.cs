// Copyright: 2023 Education Nexus Oregon
// Author: Makoa Jacobsen, makoa@makoajacobsen.com

namespace OregonNexus.Broker.Domain;

public class User : BaseEntity, IAggregateRoot
{
    public bool IsSuperAdmin { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    
    public List<UserRole>? UserRoles { get; set; }
}
