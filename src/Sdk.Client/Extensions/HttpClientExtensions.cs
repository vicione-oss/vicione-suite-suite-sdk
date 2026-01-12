namespace Sdk.Client.Extensions;

public static class HttpClientExtensions
{
    public static UriBuilder CreateUriBuilder(this HttpClient httpClient)
    {
        UriBuilder builder = new();
        if (httpClient.BaseAddress is not null)
            builder = new(httpClient.BaseAddress);
        return builder;
    }
}
