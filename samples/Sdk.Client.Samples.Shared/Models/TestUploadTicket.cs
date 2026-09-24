using Sdk.Client.Models;

namespace Sdk.Client.Samples.Shared.Models;

// A ticket stands for one upload: SettingsFieldFileUpload creates one per picked file, passes its token to the upload
// handler and calls Cancel to abort it. The type also serves as the marker T that pairs the upload field with its
// IStreamUploadHandler. The field disposes its current ticket when it is disposed itself.
internal sealed class TestUploadTicket : IUploadTicket, IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public CancellationToken CancellationToken => _cancellationTokenSource.Token;

    public void Cancel() => _cancellationTokenSource.Cancel();

    public void Dispose() => _cancellationTokenSource.Dispose();
}
