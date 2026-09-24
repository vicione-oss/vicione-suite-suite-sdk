namespace Sdk.Client.Infrastructure;

/// <summary>
/// Provides an <see cref="HttpClient"/> that bypasses the proxy settings and reaches the server over the loopback address.
/// </summary>
public interface ILocalHttpClient
{
    /// <summary>
    /// Gets the preconfigured client for requests to the local server.
    /// </summary>
    HttpClient Client { get; }
}
