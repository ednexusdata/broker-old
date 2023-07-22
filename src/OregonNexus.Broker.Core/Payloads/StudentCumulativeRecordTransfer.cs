using OregonNexus.Broker.Core.PayloadContentTypes;

namespace OregonNexus.Broker.Core.Payloads;

public class StudentCumulativeRecordTransfer : IPayload
{
    public List<IPayloadContentType> PayloadContents { get; set; }
}