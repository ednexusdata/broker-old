// Copyright: 2023 Education Nexus Oregon
// Author: Makoa Jacobsen, makoa@makoajacobsen.com

namespace OregonNexus.Broker.Domain;

public class EducationOrganizationConnectorSettings : BaseEntity, IAggregateRoot
{
    public EducationOrganization? EducationOrganization { get; set; }
    public Guid? EducationOrganizationId { get; set; }
    public string Connector { get; set; } = default!;
    public string? Settings { get; set; } = default!;
}
