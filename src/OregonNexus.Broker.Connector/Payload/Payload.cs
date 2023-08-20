using OregonNexus.Broker.Connector.PayloadContentTypes;

namespace OregonNexus.Broker.Connector.Payload;

public abstract class Payload : IPayload
{
    public List<PayloadContentType> PayloadContents;
}
