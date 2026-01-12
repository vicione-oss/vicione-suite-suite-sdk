namespace Sdk.Messaging;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class MessageEndpointAttribute(string endpointName) : Attribute
{
    public string EndpointName { get; } = endpointName;
}
