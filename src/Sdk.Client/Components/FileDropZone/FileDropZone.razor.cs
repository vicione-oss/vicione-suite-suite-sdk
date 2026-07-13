using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Sdk.Client.Extensions;

namespace Sdk.Client.Components.FileDropZone;

/// <summary>
/// A component that allows a user to drop a file, for example, to upload it.
/// </summary>
public sealed partial class FileDropZone : ComponentBase, IFileDropZone, IAsyncDisposable
{
    private bool _disposedAsync;

    private ElementReference? _elementReference;
    private ElementReference? _inputFileElementReference;

    private IJSObjectReference? _jsModule;
    private DotNetObjectReference<FileDropZone>? _dotNetObjectReference;
    private Task? _attachJsTask;
    private IJSObjectReference? _jsObjectReference;
    private bool _jsSetInputFile;

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    [Inject] private ILogger<FileDropZone> Logger { get; set; } = default!;

    /// <summary>
    /// Gets or sets the CSS class to apply to the drop zone.
    /// </summary>
    [Parameter] public string? CssClass { get; set; }

    /// <summary>
    /// Gets or sets the content to render inside the drop zone.
    /// </summary>
    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; }
    /// <summary>
    /// Raised when an <b>accepted</b> drag enters the drop zone
    /// </summary>
    [Parameter] public EventCallback OnDragEnter { get; set; }
    /// <summary>
    /// Raised when an <b>accepted</b> drag leaves the drop zone
    /// </summary>
    [Parameter] public EventCallback OnDragLeave { get; set; }
    /// <summary>
    /// Raised when a file has been dropped
    /// </summary>
    [Parameter] public EventCallback OnDrop { get; set; }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_attachJsTask is null)
        {
            _attachJsTask = AttachJsAsync();

            await _attachJsTask;
        }

        if (_jsSetInputFile && _jsObjectReference is not null)
        {
            try
            {
                await _jsObjectReference.InvokeVoidAsync("setInputFile", _inputFileElementReference);

                _jsSetInputFile = false;
            }
            catch (JSDisconnectedException)
            {
                // https://learn.microsoft.com/en-us/aspnet/core/blazor/javascript-interoperability#javascript-interop-calls-without-a-circuit
            }
            catch (Exception exception)
            {
                JsSetInputFileFailed(Logger, exception);
            }
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = $"setInputFile() failed")]
    private static partial void JsSetInputFileFailed(ILogger logger, Exception ex);

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.CompareExchange(ref _disposedAsync, true, false))
            return;

        await RemoveJsAsync();
        _dotNetObjectReference?.Dispose();
        _dotNetObjectReference = null;

        await _jsModule.TryDisposeAsync(Logger);
    }

    /// <summary>
    /// Sets the reference to the input file element.
    /// </summary>
    /// <param name="inputFileElementReference">The reference to the input file element.</param>
    public void SetInputFileElementReference(ElementReference? inputFileElementReference)
    {
        if (_inputFileElementReference?.Id != inputFileElementReference?.Id)
        {
            _inputFileElementReference = inputFileElementReference;

            _jsSetInputFile = true;

            InvokeAsync(StateHasChanged);
        }
    }

    private async Task AttachJsAsync()
    {
        if (_jsObjectReference is not null)
            return;

        _jsModule ??= await JsRuntime.InvokeAsync<IJSObjectReference>("import",
            $"./_content/{typeof(FileDropZone).Assembly.GetName().Name}/js/file-drop-zone.js");

        if (_elementReference is null)
            return;

        _dotNetObjectReference ??= DotNetObjectReference.Create(this);

        _jsObjectReference = await _jsModule!.InvokeConstructorAsync("FileDropZone", _elementReference, _dotNetObjectReference,
            _inputFileElementReference);

        _jsSetInputFile = false;
    }

    private Task RemoveJsAsync()
        => DisposeJsAttachResultAsync();

    private async Task DisposeJsAttachResultAsync()
    {
        await _jsObjectReference.TryInvokeVoidAsync("dispose", Logger);
        await _jsObjectReference.TryDisposeAsync(Logger);

        _jsObjectReference = null;
    }

    /// <summary>
    /// Invokes the event "OnDragEnter"
    /// </summary>
    /// <returns></returns>
    [JSInvokable]
    public async Task DragEnter()
    {
        if (OnDragEnter.HasDelegate)
            await OnDragEnter.InvokeAsync();
    }

    /// <summary>
    /// Invokes the event "OnDragLeave"
    /// </summary>
    /// <returns></returns>
    [JSInvokable]
    public async Task DragLeave()
    {
        if (OnDragLeave.HasDelegate)
            await OnDragLeave.InvokeAsync();
    }

    /// <summary>
    /// Invokes the event "OnDrop"
    /// </summary>
    /// <returns></returns>
    [JSInvokable]
    public async Task Drop()
    {
        if (OnDrop.HasDelegate)
            await OnDrop.InvokeAsync();
    }
}
