namespace Sdk.Client.Infrastructure;

/// <summary>
/// Marker interface to allow injection of a local HttpClient that is configured to bypass
/// the proxy settings and use the loopback address for local communication with the server.
/// </summary>
public interface ILocalHttpClient;
