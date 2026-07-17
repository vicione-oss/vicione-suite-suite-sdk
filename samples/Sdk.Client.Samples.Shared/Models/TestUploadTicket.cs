using Sdk.Client.Models;

namespace Sdk.Client.Samples.Shared.Models;

internal sealed class TestUploadTicket : IUploadTicket
{
    public CancellationToken CancellationToken { get; set; }

    public void Cancel() { }
}
