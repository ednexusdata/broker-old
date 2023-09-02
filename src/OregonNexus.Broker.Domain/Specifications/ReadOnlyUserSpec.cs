using Ardalis.Specification;

namespace OregonNexus.Broker.Domain.Specifications;

public class ReadOnlyUserSpec : Specification<User>
{
  public ReadOnlyUserSpec(Guid id)
  {
    Query
        .Include(x => x.UserRoles!)
        .ThenInclude(x => x.EducationOrganization)
        .Where(i => i.Id == id)
        .AsNoTracking();
  }
}