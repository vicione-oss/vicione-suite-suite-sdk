namespace Sdk.Client.Models;

/// <summary>
/// Represents a ticket for an upload operation, allowing for cancellation and tracking of the upload process.
/// </summary>
public interface IUploadTicket
{
    /// <summary>
    /// Gets the cancellation token associated with the upload operation.
    /// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Cancels the upload operation associated with this ticket.
    /// </summary>
    void Cancel();
}
