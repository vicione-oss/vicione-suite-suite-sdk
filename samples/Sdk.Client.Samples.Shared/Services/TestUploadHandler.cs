using Sdk.Client.Models;
using Sdk.Services;

namespace Sdk.Client.Samples.Shared.Services;

internal class TestUploadHandler : IStreamUploadHandler
{
    public Func<IStreamUploadProgress, Task>? OnProgress { get; set; }

    public async Task<IStreamUploadResult> Execute(Stream stream, string filename, CancellationToken cancellationToken = default)
    {
        await Task.Delay(1000, cancellationToken); // Simulate some processing time
        return new StreamUploadSuccessResult(filename);
    }
}
