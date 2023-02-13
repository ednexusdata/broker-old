namespace OregonNexus.Broker.SharedKernel;

public interface IHandle<T> where T : BaseDomainEvent
{
    Task HandleAsync(T args);
}