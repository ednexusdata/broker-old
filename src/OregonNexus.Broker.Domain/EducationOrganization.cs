// Copyright: 2023 Education Nexus Oregon
// Author: Makoa Jacobsen, makoa@makoajacobsen.com

namespace OregonNexus.Broker.Domain;

public class EducationOrganization : BaseEntity, IAggregateRoot
{
    public EducationOrganization? ParentOrganization { get; set; }
    public Guid? ParentOrganizationId { get; set; }
    public string Name { get; set; } = default!;
    public string? Number { get; set; } = default!;
    public EducationOrganizationType EducationOrganizationType { get; set; } = EducationOrganizationType.District;
}

public enum EducationOrganizationType
{
    District,
    School
}