namespace Sdk.Connections.Contracts;

public sealed class AzureIotHubConnection : IConnection
{
    public string Hostname { get; set; } = string.Empty;
    public string SharedAccessSignatureKey { get; set; } = string.Empty;
    public string SharedAccessSignatureKeyName { get; set; } = string.Empty;
}
