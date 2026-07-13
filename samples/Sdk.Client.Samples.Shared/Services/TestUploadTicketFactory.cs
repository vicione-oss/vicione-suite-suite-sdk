using Sdk.Client.Models;
using Sdk.Client.Samples.Shared.Models;
using Sdk.Client.Services;

namespace Sdk.Client.Samples.Shared.Services;

public class TestUploadTicketFactory : IUploadTicketFactory
{
    public IUploadTicket CreateUploadTicket() => new TestUploadTicket();
}
