namespace Sdk.Connections.Contracts;

public sealed class HttpConnection : IConnection
{
    public string BaseAddress { get; set; } = string.Empty;

    public string? ApiKey { get; set; }
}
