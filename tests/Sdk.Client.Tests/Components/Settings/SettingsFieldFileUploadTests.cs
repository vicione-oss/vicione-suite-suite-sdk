using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Forms;
using NSubstitute;
using Sdk.Client.Components.Settings;
using Sdk.Client.Contracts;
using Sdk.Client.Models;
using Sdk.Client.Services;
using Xunit;

namespace Sdk.Client.Tests.Components.Settings;

public sealed class SettingsFieldFileUploadTests
{
    [Fact]
    public async Task Should_not_cancel_a_running_upload_when_the_bound_filename_comes_back()
    {
        // Arrange
        await using var ctx = CreateContext();
        var uploadTicketFactory = new UploadTicketFactory();
        var uploadHandler = new PendingUploadHandler();
        string? boundFilename = null;

        var component = ctx.Render<SettingsFieldFileUpload<UploadMarker>>(b => b
            .Add(p => p.Filename, boundFilename)
            .Add(p => p.FilenameChanged, filename => boundFilename = filename)
            .Add(p => p.UploadTicketFactory, uploadTicketFactory)
            .Add(p => p.UploadHandler, uploadHandler));

        var upload = PickFile(component, "settings.json");

        // Act
        component.Render(b => b.Add(p => p.Filename, boundFilename));

        // Assert
        boundFilename.Should().Be("settings.json");
        uploadHandler.CancellationToken.IsCancellationRequested.Should().BeFalse();

        uploadHandler.Complete();
        await upload;
    }

    [Fact]
    public async Task Should_cancel_a_running_upload_when_the_filename_parameter_changes()
    {
        // Arrange
        await using var ctx = CreateContext();
        var uploadTicketFactory = new UploadTicketFactory();
        var uploadHandler = new PendingUploadHandler();

        var component = ctx.Render<SettingsFieldFileUpload<UploadMarker>>(b => b
            .Add(p => p.Filename, null)
            .Add(p => p.UploadTicketFactory, uploadTicketFactory)
            .Add(p => p.UploadHandler, uploadHandler));

        var upload = PickFile(component, "settings.json");

        // Act
        component.Render(b => b.Add(p => p.Filename, "other.json"));

        // Assert
        uploadHandler.CancellationToken.IsCancellationRequested.Should().BeTrue();

        await upload;
    }

    [Fact]
    public async Task Should_not_cancel_a_running_upload_when_the_ticket_from_on_upload_start_comes_back()
    {
        // Arrange
        await using var ctx = CreateContext();
        var uploadTicketFactory = new UploadTicketFactory();
        var uploadHandler = new PendingUploadHandler();
        string? boundFilename = null;
        IUploadTicket? keptUploadTicket = null;

        // Like the callers in suite, cluster-mgmt and dx: the ticket from OnUploadStart is kept and passed back.
        var component = ctx.Render<SettingsFieldFileUpload<UploadMarker>>(b => b
            .Add(p => p.Filename, boundFilename)
            .Add(p => p.FilenameChanged, filename => boundFilename = filename)
            .Add(p => p.UploadTicket, keptUploadTicket)
            .Add(p => p.OnUploadStart, uploadTicket => keptUploadTicket = uploadTicket)
            .Add(p => p.UploadTicketFactory, uploadTicketFactory)
            .Add(p => p.UploadHandler, uploadHandler));

        var upload = PickFile(component, "settings.json");

        // Act
        component.Render(b => b
            .Add(p => p.Filename, boundFilename)
            .Add(p => p.UploadTicket, keptUploadTicket));

        // Assert
        keptUploadTicket.Should().NotBeNull();
        uploadHandler.CancellationToken.IsCancellationRequested.Should().BeFalse();

        uploadHandler.Complete();
        await upload;
    }

    [Fact]
    public async Task Should_cancel_a_running_upload_when_the_parent_passes_another_ticket()
    {
        // Arrange
        await using var ctx = CreateContext();
        var uploadTicketFactory = new UploadTicketFactory();
        var uploadHandler = new PendingUploadHandler();
        using var otherUploadTicket = new UploadTicket();

        var component = ctx.Render<SettingsFieldFileUpload<UploadMarker>>(b => b
            .Add(p => p.UploadTicketFactory, uploadTicketFactory)
            .Add(p => p.UploadHandler, uploadHandler));

        var upload = PickFile(component, "settings.json");

        // Act
        component.Render(b => b.Add(p => p.UploadTicket, otherUploadTicket));

        // Assert
        uploadHandler.CancellationToken.IsCancellationRequested.Should().BeTrue();

        await upload;
    }

    private static BunitContext CreateContext()
    {
        var ctx = new BunitContext();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;

        return ctx;
    }

    private static Task PickFile(IRenderedComponent<SettingsFieldFileUpload<UploadMarker>> component, string filename)
    {
        var browserFile = Substitute.For<IBrowserFile>();
        browserFile.Name.Returns(filename);
        browserFile.OpenReadStream(Arg.Any<long>(), Arg.Any<CancellationToken>()).Returns(_ => new MemoryStream([1, 2, 3]));

        // The second InputFile is the one the choose button opens; the first one belongs to the drop zone.
        var filePicker = component.FindComponents<InputFile>()[1];

        return filePicker.InvokeAsync(() => filePicker.Instance.OnChange.InvokeAsync(new InputFileChangeEventArgs([browserFile])));
    }

    private sealed class UploadMarker;

    private sealed class UploadTicket : IUploadTicket, IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public CancellationToken CancellationToken => _cancellationTokenSource.Token;

        public void Cancel() => _cancellationTokenSource.Cancel();

        public void Dispose() => _cancellationTokenSource.Dispose();
    }

    private sealed class UploadTicketFactory : IUploadTicketFactory
    {
        public IUploadTicket CreateUploadTicket() => new UploadTicket();
    }

    private sealed class PendingUploadHandler : IStreamUploadHandler<UploadMarker>
    {
        private readonly TaskCompletionSource _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Func<IStreamUploadProgress, Task>? OnProgress { get; set; }

        public CancellationToken CancellationToken { get; private set; }

        public void Complete() => _completion.TrySetResult();

        public async Task<IStreamUploadResult> Execute(Stream stream, string filename, CancellationToken cancellationToken = default)
        {
            CancellationToken = cancellationToken;

            await _completion.Task.WaitAsync(cancellationToken);

            return new StreamUploadSuccessResult(filename);
        }
    }
}
