namespace OregonNexus.Broker.SharedKernel;

public interface ICurrentUser
{
    public Guid? AuthenticatedUserId();
}