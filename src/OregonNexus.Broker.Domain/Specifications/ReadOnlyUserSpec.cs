using Ardalis.Specification;

namespace OregonNexus.Broker.Domain.Specifications;

public class ReadOnlyUserSpec : Specification<User>
{
  public ReadOnlyUserSpec(Guid id)
  {
    Query
        .Where(i => i.Id == id)
        .AsNoTracking();
  }
}