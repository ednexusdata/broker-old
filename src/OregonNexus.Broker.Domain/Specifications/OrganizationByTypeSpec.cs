using Ardalis.Specification;

namespace OregonNexus.Broker.Domain.Specifications;

public class OrganizationByTypeSpec : Specification<EducationOrganization>, ISingleResultSpecification
{
  public OrganizationByTypeSpec(EducationOrganizationType orgType)
  {
    Query
        .Where(organizationType => organizationType.EducationOrganizationType == orgType);
  }
}