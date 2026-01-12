namespace Sdk.Messaging;

/// <summary>
/// Specifies the dedicated endpoint name for a message contract.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class MessageEndpointAttribute(string endpointName) : Attribute
{
    /// <summary>
    /// Gets the name of the message endpoint.
    /// </summary>
    public string EndpointName { get; } = endpointName;
}
