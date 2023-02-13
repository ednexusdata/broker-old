using Ardalis.Specification;

namespace OregonNexus.Broker.SharedKernel;

public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
{
}