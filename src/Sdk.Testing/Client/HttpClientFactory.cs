using System.Text;
using System.Text.Json;
using NSubstitute;

namespace Sdk.Testing.Client;

public static class HttpClientFactory
{
    private static readonly Uri s_baseUri = new("http://localhost");

    private static readonly JsonSerializerOptions s_jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static HttpClient GetHttpClientWithResponse(object response, Uri? baseUri = null)
    {
        var httpMock = GetHttpClientHandlerJsonMock(response);
        return new HttpClient(httpMock)
        {
            BaseAddress = baseUri ?? s_baseUri
        };
    }

    private static HttpMessageHandlerMock GetHttpClientHandlerJsonMock(object obj)
    {
        if (obj is string str && string.IsNullOrEmpty(str))
        {
            return Substitute.ForPartsOf<HttpMessageHandlerMock>(new StringContent(string.Empty, Encoding.UTF8, "application/json"));
        }

        var jsonContent = JsonSerializer.Serialize(obj, s_jsonSerializerOptions);
        return Substitute.ForPartsOf<HttpMessageHandlerMock>(new StringContent(jsonContent, Encoding.UTF8, "application/json"));
    }
}
