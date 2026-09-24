using System.Net;

namespace Sdk.Testing.Client;

/// <summary>
/// Answers every request with 200 OK and a copy of <paramref name="responseContent"/>'s body and headers.
/// </summary>
/// <param name="responseContent">The response template; read once on the first request and disposed with the handler.</param>
public class HttpMessageHandlerMock(HttpContent responseContent) : HttpMessageHandler
{
    private readonly HttpContent _responseContent = responseContent;

    // Each response gets its own content: disposing a response disposes its content, which broke every later request.
    private readonly Lazy<byte[]> _body = new(() =>
    {
        using var stream = responseContent.ReadAsStream();
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        return buffer.ToArray();
    });

    /// <inheritdoc/>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => Task.FromResult(Send(request, cancellationToken));

    /// <inheritdoc/>
    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        => new()
        {
            StatusCode = HttpStatusCode.OK,
            Content = CreateContent(),
        };

    private ByteArrayContent CreateContent()
    {
        var content = new ByteArrayContent(_body.Value);
        foreach (var header in _responseContent.Headers)
            content.Headers.TryAddWithoutValidation(header.Key, header.Value);

        return content;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _responseContent.Dispose();
        }
        base.Dispose(disposing);
    }
}
