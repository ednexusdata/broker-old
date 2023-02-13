using Ardalis.Specification;

namespace OregonNexus.Broker.SharedKernel;

public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
{
    
}