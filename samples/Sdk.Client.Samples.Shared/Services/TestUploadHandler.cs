using Sdk.Client.Contracts;
using Sdk.Client.Models;
using Sdk.Client.Samples.Shared.Models;

namespace Sdk.Client.Samples.Shared.Services;

internal sealed class TestUploadHandler : IStreamUploadHandler<TestUploadTicket>
{
    public Func<IStreamUploadProgress, Task>? OnProgress { get; set; }

    public async Task<IStreamUploadResult> Execute(Stream stream, string filename, CancellationToken cancellationToken = default)
    {
        await Task.Delay(1000, cancellationToken); // Simulate some processing time
        return new StreamUploadSuccessResult(filename);
    }
}
