namespace Sdk.Client.Infrastructure;

/// <summary>
/// Interface to allow injection of a pre-configured HttpClient to bypass
/// the proxy settings and use the loopback address for local communication with the server.
/// </summary>
public interface ILocalHttpClient
{
    /// <summary>
    /// The pre-configured http client instance to use for requests
    /// </summary>
    HttpClient Client { get; }
}
