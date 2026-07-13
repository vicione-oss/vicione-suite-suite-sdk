using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Sdk.Client.Extensions;
using Sdk.Client.Models;
using Sdk.Client.Services;
using Sdk.Services;
using ViciOne.Ui.Localization.Resources;
using ViciOne.Ui.MonochromeIcons.Core.Enums;
using ViciOne.Ui.MonochromeIcons.Core.Extensions;

namespace Sdk.Client.Components.Settings;

/// <summary>
/// A component that provides a file upload field with drag-and-drop support and progress tracking.
/// </summary>
public sealed partial class SettingsFieldFileUpload : ComponentBase, IAsyncDisposable
{
    private readonly string _chooseButtonIconCssClasses = MonochromeIconName.Folder.GetCssClasses(MonochromeIconSize.Small).ToSpaceSeparated();
    private readonly string _cancelButtonIconCssClasses = MonochromeIconName.CloseMedium.GetCssClasses(MonochromeIconSize.Small).ToSpaceSeparated();

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private IJSObjectReference? _jsModule;
    private DotNetObjectReference<SettingsFieldFileUpload>? _dotNetObjectReference;
    private Task? _attachJsTask;
    private IJSObjectReference? _jsObjectReference;

    private bool _disposedAsync;

    private FileDropZone.FileDropZone? _fileDropZone;
    private InputFile? _inputFile;
    private InputFile? _inputFileForShowPicker;

    private string? _placeholder;

    private string? _filename;
    private IStreamUploadHandler? _uploadHandler;
    private IStreamUploadResult? _uploadResult;
    private int? _uploadProgress;
    private IUploadTicket? _uploadTicket;
    private IUploadTicketFactory? _uploadTicketFactory;
    private readonly SemaphoreSlim _uploadTicketSemaphore = new(1);
    private bool _shouldRender = true;

    private bool _dropIncoming;

    private string ChooseButtonText => _uploadTicket is not null && _uploadProgress is not null
        ? CommonVocabulary.Cancel : CommonVocabulary.Choose;

    private string ChooseButtonIcon => _uploadTicket is not null && _uploadProgress is not null
        ? _cancelButtonIconCssClasses : _chooseButtonIconCssClasses;

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    [Inject] private ILogger<FileDropZone.FileDropZone> Logger { get; set; } = default!;

    /// <summary>
    /// Value rendered into <see href="https://html.spec.whatwg.org/#attr-input-accept">accept</see> attribute of internal input fields
    /// </summary>
    [Parameter]
    public string? Accept { get; set; }

    /// <summary>
    /// Maximum allowed size of the uploaded file in bytes. Default is 500 KB.
    /// </summary>
    [Parameter]
    public long MaximumAllowedSize { get; set; } = 500 * 1024;

    /// <summary>
    /// Placeholder text displayed in the input field when no file is selected.
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
    public IStreamUploadHandler UploadHandler { get; set; }

    /// <summary>
    /// Gets or sets the content to render inside the component.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Invoked when the upload process starts.
    /// </summary>
    [Parameter]
    public EventCallback<IUploadTicket> OnUploadStart { get; set; }

    /// <summary>
    /// Invoked when the upload process completes successfully.
    /// </summary>
    [Parameter]
    public EventCallback<StreamUploadSuccessResult> OnUploadSuccess { get; set; }

    /// <summary>
    /// Invoked when the upload process encounters an error.
    /// </summary>
    [Parameter]
    public EventCallback<StreamUploadErrorResult> OnUploadError { get; set; }

    /// <summary>
    /// Invoked when the upload process is canceled.
    /// </summary>
    [Parameter]
    public EventCallback OnUploadCancel { get; set; }

    private async Task AttachJsAsync()
    {
        if (_jsObjectReference is not null)
            return;

        _jsModule ??= await JsRuntime.InvokeAsync<IJSObjectReference>("import",
            $"./_content/{typeof(SettingsFieldFileUpload).Assembly.GetName().Name}/js/settings-field-file-upload.js");

        _dotNetObjectReference ??= DotNetObjectReference.Create(this);

        _jsObjectReference = await _jsModule!.InvokeConstructorAsync("SettingsFieldFileUpload");
    }

    /// <summary>
    /// Called when the component's parameters are set.
    /// </summary>
    /// <returns>Asyncronous Task</returns>
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

            if (UploadTicket != _uploadTicket)
            {
                _uploadTicket = UploadTicket;

                cancelUpload = true;
            }
            else if (updatedFactory)
            {
                _uploadTicket = _uploadTicketFactory?.CreateUploadTicket();
                UploadTicket = _uploadTicket;
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
    /// Releases all resources used by the component
    /// </summary>
    /// <returns>async Task</returns>
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

    /// <summary>
    /// Called after the component has been rendered.
    /// </summary>
    /// <param name="firstRender"></param>
    /// <returns>async Task</returns>
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
    /// Determines whether the component should be rendered.
    /// </summary>
    /// <returns>True if the component should be rendered, otherwise false.</returns>
    protected override bool ShouldRender()
    {
        if (_shouldRender)
        {
            _shouldRender = false;

            return true;
        }

        return false;
    }

    private async Task FileActionButtonClick()
    {
        // handle cancel upload if in progress, otherwise show file picker
        if (_uploadTicket is not null && _uploadProgress is not null)
        {
            try
            {
                _uploadTicket.Cancel();
                //_uploadTicket = null;
                _uploadProgress = null;
                _uploadResult = null;
                _filename = null;
                _shouldRender = true;
            }
            finally
            {
                _uploadTicketSemaphore.Release();
            }
            return;
        }

        // show file picker dialog via JS interop
        if (_jsObjectReference is not null)
        {
            try
            {
                await _jsObjectReference.InvokeVoidAsync("showFilePicker", _inputFileForShowPicker?.Element);
            }
            catch (JSDisconnectedException)
            {
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

    private async Task FileDropped(InputFileChangeEventArgs args)
    {
        if (!string.IsNullOrWhiteSpace(Accept))
        {
            var acceptTokens = Accept.Split(',');

            var fileExtension = Path.GetExtension(args.File.Name);

            if (!acceptTokens.Any(acceptToken => string.Equals(acceptToken, fileExtension, StringComparison.OrdinalIgnoreCase)))
                return;
        }

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
            // CancellationTokenSource already disposed, return gracefully
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
