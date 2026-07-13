using Sdk.Client.Models;

namespace Sdk.Client.Services;

/// <summary>
/// Factory interface for creating instances of <see cref="IUploadTicket"/>.
/// </summary>
public interface IUploadTicketFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="IUploadTicket"/>.
    /// </summary>
    /// <returns></returns>
    IUploadTicket CreateUploadTicket();
}
