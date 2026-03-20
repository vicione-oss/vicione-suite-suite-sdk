using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Sdk.Client.Extensions;
using Sdk.Client.Services;

namespace Sdk.Client.Components.FileDrop;

/// <summary>
/// A component that provides a drag-and-drop zone for file uploads.
/// </summary>
public sealed partial class FileDropComponent
{
    private InputFile? _inputFile;
    private ElementReference? _dropZoneElement;
    private ElementReference? _inputLabel;
    private ElementReference? _descriptionLabel;
    private bool _isInitialized;
    private IJSObjectReference? _fileDrop;

    [Inject] private IJsInterop JsInterop { get; set; } = default!;

    [Inject] private ILogger<FileDropComponent> Logger { get; set; } = default!;

    /// <summary>
    /// Gets or sets a callback that is invoked when the user selects or drops files.
    /// </summary>
    [Parameter]
    public EventCallback<InputFileChangeEventArgs> OnChange { get; set; }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!_isInitialized)
        {
            var jsObjectReference = await JsInterop.IncludeModuleScript(new Uri("./_content/ViciOne.Suite.Sdk.Client/js/file-drop-component.js", UriKind.Relative));

            if (_inputFile?.Element is not null &&
                _dropZoneElement is not null &&
                _inputLabel is not null &&
                _descriptionLabel is not null &&
                jsObjectReference is not null &&
                _fileDrop is null)
            {
                _isInitialized = true;

                _fileDrop = await jsObjectReference.InvokeConstructorAsync("FileDrop", _dropZoneElement, _inputLabel, _descriptionLabel, Localization.FileDropComponent.NoFileSelected, _inputFile.Element);
            }
        }
    }

    /// <summary>
    /// Releases all resources used by the component, including the JavaScript interop object.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await _fileDrop.TryInvokeVoidAsync("dispose", Logger);
        await _fileDrop.TryDisposeAsync(Logger);

        _isInitialized = false;
    }
}
