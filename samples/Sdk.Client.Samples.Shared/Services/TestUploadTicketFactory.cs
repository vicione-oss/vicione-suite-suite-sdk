using Sdk.Client.Models;
using Sdk.Client.Samples.Shared.Models;
using Sdk.Client.Services;

namespace Sdk.Client.Samples.Shared.Services;

// SettingsFieldFileUpload asks this factory for a fresh ticket for every upload it starts.
public class TestUploadTicketFactory : IUploadTicketFactory
{
    public IUploadTicket CreateUploadTicket() => new TestUploadTicket();
}
