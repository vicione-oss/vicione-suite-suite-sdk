using Sdk.Client.Models;

namespace Sdk.Client.Samples.Shared.Models;

internal class TestUploadTicket : IUploadTicket
{
    public CancellationToken CancellationToken { get; set; }

    public void Cancel() { }
}
