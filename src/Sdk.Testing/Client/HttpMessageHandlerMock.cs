using System.Net;

namespace Sdk.Testing.Client;

/// <summary>
/// A mock implementation of <see cref="HttpMessageHandler"/> for testing purposes.
/// </summary>
public class HttpMessageHandlerMock(HttpContent responseContent) : HttpMessageHandler
{
    private readonly HttpContent _responseContent = responseContent;

    /// <summary>
    /// Sends an HTTP request as an asynchronous operation, returning a predefined response.
    /// </summary>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => Task.FromResult(Send(request, cancellationToken));

    /// <summary>
    /// Sends an HTTP request, returning a predefined response.
    /// </summary>
    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        => new()
        {
            StatusCode = HttpStatusCode.OK,
            Content = _responseContent,
        };

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="HttpMessageHandlerMock"/> and optionally disposes of the managed resources.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _responseContent.Dispose();
        }
        base.Dispose(disposing);
    }
}
