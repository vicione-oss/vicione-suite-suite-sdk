using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Sdk.Client.Contracts;
using Sdk.Client.Extensions;
using Sdk.Client.Models;
using Sdk.Client.Services;
using ViciOne.Ui.Localization.Resources;
using ViciOne.Ui.MonochromeIcons.Core.Enums;
using ViciOne.Ui.MonochromeIcons.Core.Extensions;

namespace Sdk.Client.Components.Settings;

/// <summary>
/// A settings field that uploads a picked or dropped file through an <see cref="IStreamUploadHandler{T}"/> and shows its progress;
/// <typeparamref name="T"/> is the marker that selects the handler.
/// </summary>
public sealed partial class SettingsFieldFileUpload<T> : ComponentBase, IAsyncDisposable
{
    private readonly string _cancelButtonIconCssClasses = MonochromeIconName.CloseMedium.GetCssClasses(MonochromeIconSize.Small).ToSpaceSeparated();

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private IJSObjectReference? _jsModule;
    private DotNetObjectReference<SettingsFieldFileUpload<T>>? _dotNetObjectReference;
    private Task? _attachJsTask;
    private IJSObjectReference? _jsObjectReference;

    private bool _disposedAsync;

    private FileDropZone.FileDropZone? _fileDropZone;
    private InputFile? _inputFile;
    private InputFile? _inputFileForShowPicker;

    private string? _placeholder;

    private string? _filename;
    private IStreamUploadHandler<T>? _uploadHandler;
    private IStreamUploadResult? _uploadResult;
    private int? _uploadProgress;
    private IUploadTicket? _uploadTicket;
    // The UploadTicket parameter as last passed; unlike _uploadTicket, it does not change when an upload starts.
    private IUploadTicket? _passedUploadTicket;
    private IUploadTicketFactory? _uploadTicketFactory;
    private readonly SemaphoreSlim _uploadTicketSemaphore = new(1);
    private bool _shouldRender = true;

    private bool _dropIncoming;

    /// <summary>
    /// Gets or sets the CSS classes of the choose button's icon, replaced by a cancel icon during an upload.
    /// Defaults to a small folder icon.
    /// </summary>
    [Parameter] public string ChooseButtonIconCssClasses { get; set; } = MonochromeIconName.Folder.GetCssClasses(MonochromeIconSize.Small).ToSpaceSeparated();

    /// <summary>
    /// Gets or sets the choose button's text, replaced by a localized "Cancel" during an upload. Defaults to a localized "Choose".
    /// </summary>
    [Parameter] public string ChooseButtonText { get; set; } = CommonVocabulary.Choose;

    private string FileButtonText => _uploadTicket is not null && _uploadProgress is not null
        ? CommonVocabulary.Cancel : ChooseButtonText;

    private string FileButtonIcon => _uploadTicket is not null && _uploadProgress is not null
        ? _cancelButtonIconCssClasses : ChooseButtonIconCssClasses;

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    [Inject] private ILogger<FileDropZone.FileDropZone> Logger { get; set; } = default!;

    /// <summary>
    /// Gets or sets the value of the inputs' <see href="https://html.spec.whatwg.org/#attr-input-accept">accept</see> attribute.
    /// A dropped file is checked against it as well: by file extension (<c>.png, .jpg</c>), MIME type (<c>image/png</c>) or wildcard
    /// (<c>image/*</c>), ignoring case and whitespace; a file that matches no token is not uploaded.
    /// </summary>
    [Parameter]
    public string? Accept { get; set; }

    /// <summary>
    /// Gets or sets the maximum file size in bytes. Defaults to 500 KiB.
    /// </summary>
    [Parameter]
    public long MaximumAllowedSize { get; set; } = 500 * 1024;

    /// <summary>
    /// Gets or sets the text displayed while no file is selected.
    /// </summary>
    [Parameter]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Gets or sets the name of the file being uploaded.
    /// </summary>
    [Parameter, EditorRequired]
    public string? Filename { get; set; }

    /// <summary>
    /// Gets or sets a callback that is invoked when the filename changes.
    /// </summary>
    [Parameter]
    public EventCallback<string?> FilenameChanged { get; set; }


    /// <summary>
    /// Gets or sets whether the filename is hidden. Defaults to <see langword="false"/>.
    /// </summary>
    [Parameter]
    public bool HideFilename { get; set; }

    /// <summary>
    /// Gets or sets the upload ticket used to manage the upload process.
    /// </summary>
    [Parameter]
    public IUploadTicket? UploadTicket { get; set; }

    /// <summary>
    /// Gets or sets the upload ticket factory used to create instances of <see cref="IUploadTicket"/>.
    /// </summary>
    [Parameter, EditorRequired]
    public IUploadTicketFactory UploadTicketFactory { get; set; }

    /// <summary>
    /// Gets or sets the upload handler responsible for handling the file upload process.
    /// </summary>
    [Parameter, EditorRequired]
    public IStreamUploadHandler<T> UploadHandler { get; set; }

    /// <summary>
    /// Gets or sets the content to render inside the component.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when an upload starts.
    /// </summary>
    [Parameter]
    public EventCallback<IUploadTicket> OnUploadStart { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when an upload succeeds.
    /// </summary>
    [Parameter]
    public EventCallback<StreamUploadSuccessResult> OnUploadSuccess { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when an upload fails, including when the handler throws.
    /// </summary>
    [Parameter]
    public EventCallback<StreamUploadErrorResult> OnUploadError { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when an upload is canceled.
    /// </summary>
    [Parameter]
    public EventCallback OnUploadCancel { get; set; }

    private async Task AttachJsAsync()
    {
        if (_jsObjectReference is not null)
            return;

        _jsModule ??= await JsRuntime.InvokeAsync<IJSObjectReference>("import",
            $"./_content/{typeof(SettingsFieldFileUpload<T>).Assembly.GetName().Name}/js/settings-field-file-upload.js");

        _dotNetObjectReference ??= DotNetObjectReference.Create(this);

        _jsObjectReference = await _jsModule!.InvokeConstructorAsync("SettingsFieldFileUpload");
    }

    /// <summary>
    /// Takes over changed parameters; a new filename, ticket, ticket factory or handler cancels the running upload.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        var uploadTicket = _uploadTicket;
        var cancelUpload = false;

        if (Placeholder != _placeholder)
        {
            _placeholder = Placeholder;

            _shouldRender = true;
        }

        if (Filename != _filename)
        {
            _filename = Filename;

            cancelUpload = true;
        }

        await _uploadTicketSemaphore.WaitAsync(_cancellationTokenSource.Token);
        try
        {
            var updatedFactory = false;

            if (UploadTicketFactory != _uploadTicketFactory)
            {
                _uploadTicketFactory = UploadTicketFactory;
                updatedFactory = true;
                cancelUpload = true;
            }

            // Only a ticket the parent changed counts: a parent that passes no ticket keeps passing null while an upload
            // replaces _uploadTicket, and a parent that keeps the ticket from OnUploadStart passes the running one back.
            if (UploadTicket != _passedUploadTicket)
            {
                _passedUploadTicket = UploadTicket;

                if (UploadTicket != _uploadTicket)
                {
                    _uploadTicket = UploadTicket;

                    cancelUpload = true;
                }
            }
            else if (updatedFactory)
            {
                _uploadTicket = _uploadTicketFactory?.CreateUploadTicket();
            }
        }
        finally
        {
            _uploadTicketSemaphore.Release();
        }

        if (UploadHandler != _uploadHandler)
        {
            _uploadHandler?.OnProgress -= UploadHandlerProgress;

            _uploadHandler = UploadHandler;
            _uploadHandler.OnProgress += UploadHandlerProgress;

            cancelUpload = true;
        }

        if (cancelUpload)
        {
            uploadTicket?.Cancel();

            _uploadProgress = null;
            _uploadResult = null;

            _shouldRender = true;
        }
    }

    /// <summary>
    /// Cancels a running upload and releases the upload ticket and the JavaScript module.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.CompareExchange(ref _disposedAsync, true, false))
            return;

        _uploadHandler?.OnProgress -= UploadHandlerProgress;

        _fileDropZone?.SetInputFileElementReference(null);

        await _cancellationTokenSource.CancelAsync();
        _cancellationTokenSource.Dispose();

        await _uploadTicketSemaphore.WaitAsync();
        try
        {
            if (_uploadTicket is IDisposable disposableUploadTicket)
                disposableUploadTicket.Dispose();
        }
        finally
        {
            _uploadTicketSemaphore.Release();
        }

        _uploadTicketSemaphore.Dispose();

        await RemoveJsAsync();
        _dotNetObjectReference?.Dispose();
        _dotNetObjectReference = null;

        await _jsModule.TryDisposeAsync(Logger);
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_attachJsTask is null)
        {
            _attachJsTask = AttachJsAsync();

            await _attachJsTask;
        }

        _fileDropZone?.SetInputFileElementReference(_inputFile?.Element);
    }

    /// <summary>
    /// Renders only when the component's state changed since the last render.
    /// </summary>
    protected override bool ShouldRender()
    {
        if (_shouldRender)
        {
            _shouldRender = false;

            return true;
        }

        return false;
    }

    /// <summary>
    /// Resets the component state so another file can be picked/uploaded.
    /// </summary>
    public async Task ResetAsync()
    {
        try
        {
            await _uploadTicketSemaphore.WaitAsync(_cancellationTokenSource.Token);
            try
            {
                _uploadTicket?.Cancel();
                _uploadTicket = _uploadTicketFactory?.CreateUploadTicket();
                _uploadProgress = null;
                _uploadResult = null;
                _filename = null;
                _shouldRender = true;
            }
            finally
            {
                _uploadTicketSemaphore.Release();
            }
        }
        catch (OperationCanceledException)
        {
            // Canceled by DisposeAsync: the component is going away, so there is nothing to reset.
        }
        catch (ObjectDisposedException)
        {
            // Disposed concurrently: the component is going away, so there is nothing to reset.
        }

        if (FilenameChanged.HasDelegate)
            await FilenameChanged.InvokeAsync(null);

        await InvokeAsync(StateHasChanged);
        await ResetInputAsync();
    }

    private async Task ResetInputAsync()
    {
        if (_jsObjectReference is not null)
        {
            try
            {
                await _jsObjectReference.InvokeVoidAsync("resetInput", _inputFile?.Element, _inputFileForShowPicker?.Element);
            }
            catch (JSDisconnectedException)
            {
                // The circuit is gone, so there is no JavaScript side left to update:
                // https://learn.microsoft.com/en-us/aspnet/core/blazor/javascript-interoperability#javascript-interop-calls-without-a-circuit
            }
            catch (Exception exception)
            {
                JsResetInputFailed(Logger, exception);
            }
        }
    }

    private async Task FileActionButtonClick()
    {
        // The button cancels a running upload; otherwise it opens the file picker.
        if (_uploadTicket is not null && _uploadProgress is not null)
        {
            await ResetAsync();
            return;
        }

        if (_jsObjectReference is not null)
        {
            try
            {
                await _jsObjectReference.InvokeVoidAsync("showFilePicker", _inputFileForShowPicker?.Element);
            }
            catch (JSDisconnectedException)
            {
                // The circuit is gone, so there is no JavaScript side left to update:
                // https://learn.microsoft.com/en-us/aspnet/core/blazor/javascript-interoperability#javascript-interop-calls-without-a-circuit
            }
            catch (Exception exception)
            {
                JsShowFilePickerFailed(Logger, exception);
            }
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = $"showFilePicker() failed")]
    private static partial void JsShowFilePickerFailed(ILogger logger, Exception ex);

    [LoggerMessage(Level = LogLevel.Error, Message = $"resetInput() failed")]
    private static partial void JsResetInputFailed(ILogger logger, Exception ex);

    private async Task FileDropped(InputFileChangeEventArgs args)
    {
        if (!AcceptFilter.Matches(Accept, args.File.Name, args.File.ContentType))
            return;

        await UploadFile(args.File);
    }

    private async Task FilePicked(InputFileChangeEventArgs args)
        => await UploadFile(args.File);

    private async Task UploadFile(IBrowserFile browserFile)
    {
        try
        {
            IUploadTicket? uploadTicket;

            await _uploadTicketSemaphore.WaitAsync(_cancellationTokenSource.Token);
            try
            {
                _uploadTicket?.Cancel();

                _uploadTicket = _uploadTicketFactory!.CreateUploadTicket();

                uploadTicket = _uploadTicket;
                _shouldRender = true;
            }
            finally
            {
                _uploadTicketSemaphore.Release();
            }

            if (OnUploadStart.HasDelegate)
                await OnUploadStart.InvokeAsync(uploadTicket);

            await using var readStream = browserFile.OpenReadStream(MaximumAllowedSize, uploadTicket.CancellationToken);

            _filename = browserFile.Name;
            _uploadProgress = null;
            _uploadResult = null;
            _shouldRender = true;

            if (FilenameChanged.HasDelegate)
                await FilenameChanged.InvokeAsync(_filename);

            _uploadResult = await UploadHandler.Execute(readStream, _filename, uploadTicket.CancellationToken);

            if (_uploadResult is StreamUploadSuccessResult successResult)
            {
                if (OnUploadSuccess.HasDelegate)
                    await OnUploadSuccess.InvokeAsync(successResult);
            }
            else if (_uploadResult is StreamUploadErrorResult errorResult)
            {
                if (OnUploadError.HasDelegate)
                    await OnUploadError.InvokeAsync(errorResult);
            }
            else
            {
                throw new NotSupportedException("Result type is not supported");
            }

            _shouldRender = true;
            await InvokeAsync(StateHasChanged);
        }
        catch (OperationCanceledException)
        {
            if (OnUploadCancel.HasDelegate)
                await OnUploadCancel.InvokeAsync();
        }
        catch (ObjectDisposedException)
        {
            // Disposed during the upload: the component is going away.
        }
        catch (Exception ex)
        {
            if (OnUploadError.HasDelegate)
                await OnUploadError.InvokeAsync(new StreamUploadErrorResult(ex.Message));

            _shouldRender = true;
            await InvokeAsync(StateHasChanged);
        }
    }

    private Task RemoveJsAsync()
        => DisposeJsAttachResultAsync();

    private async Task DisposeJsAttachResultAsync()
    {
        await _jsObjectReference.TryDisposeAsync(Logger);

        _jsObjectReference = null;
    }

    private async Task UploadHandlerProgress(IStreamUploadProgress progress)
    {
        var uploadProgress = _uploadProgress;

        _uploadProgress = (int)Math.Floor(progress.BytesUploaded * 100.0 / progress.BytesTotal);

        if (_uploadProgress != uploadProgress)
        {
            _shouldRender = true;

            await InvokeAsync(StateHasChanged);
        }
    }

    private void FileDropZoneDragEnter()
        => SetDropIncoming(true);

    private void FileDropZoneDragLeave()
        => SetDropIncoming(false);

    private void FileDropZoneDrop()
        => SetDropIncoming(false);

    private void SetDropIncoming(bool value)
    {
        if (_dropIncoming != value)
        {
            _dropIncoming = value;

            _shouldRender = true;

            InvokeAsync(StateHasChanged);
        }
    }
}
