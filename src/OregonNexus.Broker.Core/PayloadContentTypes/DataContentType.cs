namespace OregonNexus.Broker.Core.PayloadContentTypes;

public class DataContentType : IPayloadContentType
{
    public string Schema { get; set; }
    public string SchemaVersion { get; set; }
}