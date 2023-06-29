using OregonNexus.Broker.Core.PayloadContentTypes;

namespace OregonNexus.Broker.Core.Payloads;

public abstract class Payload : IPayload
{
    public List<IPayloadContentType> PayloadContents;
}
