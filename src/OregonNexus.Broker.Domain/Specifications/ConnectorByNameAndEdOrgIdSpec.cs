using Ardalis.Specification;

namespace OregonNexus.Broker.Domain.Specifications;

public class ConnectorByNameAndEdOrgIdSpec : Specification<EducationOrganizationConnectorSettings>
{
  public ConnectorByNameAndEdOrgIdSpec(string connectorName, Guid educationOrganizationId)
  {
    Query
        .Where(x => x.Connector == connectorName && x.EducationOrganizationId == educationOrganizationId);
  }
}