using System.Net;

namespace Sdk.Testing.Client;

public class HttpMessageHandlerMock(HttpContent responseContent) : HttpMessageHandler
{
    private readonly HttpContent _responseContent = responseContent;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => Task.FromResult(Send(request, cancellationToken));

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        => new()
        {
            StatusCode = HttpStatusCode.OK,
            Content = _responseContent,
        };

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _responseContent.Dispose();
        }
        base.Dispose(disposing);
    }
}
