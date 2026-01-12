namespace Sdk.Client.Extensions;

/// <summary>
/// Provides extension methods for <see cref="HttpClient"/>.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Creates a new <see cref="UriBuilder"/>, optionally initialized with the client's <see cref="HttpClient.BaseAddress"/>.
    /// </summary>
    public static UriBuilder CreateUriBuilder(this HttpClient httpClient)
    {
        UriBuilder builder = new();
        if (httpClient.BaseAddress is not null)
            builder = new(httpClient.BaseAddress);
        return builder;
    }
}
