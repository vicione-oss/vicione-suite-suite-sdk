using Sdk.Client.Contracts;
using Sdk.Client.Models;
using Sdk.Client.Samples.Shared.Models;

namespace Sdk.Client.Samples.Shared.Services;

// Stands in for the handler a module registers with AddStreamUploadHandler, which streams the file to the backend.
// Like a real handler, it reads the stream in chunks and invokes OnProgress after each one; SettingsFieldFileUpload
// turns those calls into its progress display. Instead of sending the bytes anywhere, it only waits a little per chunk.
internal sealed class TestUploadHandler : IStreamUploadHandler<TestUploadTicket>
{
    private const int ChunkSize = 16 * 1024;

    public Func<IStreamUploadProgress, Task>? OnProgress { get; set; }

    public async Task<IStreamUploadResult> Execute(Stream stream, string filename, CancellationToken cancellationToken = default)
    {
        var buffer = new byte[ChunkSize];
        var bytesTotal = stream.Length;
        var bytesUploaded = 0L;
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, cancellationToken)) > 0)
        {
            bytesUploaded += bytesRead;

            if (OnProgress is not null)
                await OnProgress(new TestUploadProgress(filename, bytesUploaded, bytesTotal));

            // Slows the loop down so the progress display can be watched; the token lets a cancel stop it at once.
            await Task.Delay(50, cancellationToken);
        }

        return new StreamUploadSuccessResult(filename);
    }

    private sealed record TestUploadProgress(string Filename, long BytesUploaded, long BytesTotal) : IStreamUploadProgress
    {
        public string Path => string.Empty;

        public string? DestinationFile => null;
    }
}
