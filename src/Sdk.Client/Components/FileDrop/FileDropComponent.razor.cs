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
    /// Gets or sets a callback that is invoked when the user selects or drops a file, or several files if <see cref="Multiple"/> is
    /// set; it is not invoked for a drop without files.
    /// </summary>
    [Parameter]
    public EventCallback<InputFileChangeEventArgs> OnChange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the component shows "No file selected" again once <see cref="OnChange"/> has
    /// finished, also when it throws; by default, the name of the last file stays.
    /// </summary>
    [Parameter]
    public bool ClearAfterChange { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user can select or drop several files at once; without it, a drop of several
    /// files is rejected with a message and <see cref="OnChange"/> is not invoked.
    /// </summary>
    [Parameter]
    public bool Multiple { get; set; }

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

                var texts = new
                {
                    Localization.FileDropComponent.NoFileSelected,
                    Localization.FileDropComponent.OnlyOneFileCanBeDropped,
                    Localization.FileDropComponent.FilesSelected
                };

                _fileDrop = await jsObjectReference.InvokeConstructorAsync("FileDrop", _dropZoneElement, _inputLabel, _descriptionLabel,
                    _inputFile.Element, texts);
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

    private async Task ForwardChange(InputFileChangeEventArgs args)
    {
        // The script already refuses such drops; this check keeps the documented promise of OnChange for any other change of the input.
        if (args.FileCount == 0 || (args.FileCount > 1 && !Multiple))
            return;

        try
        {
            await OnChange.InvokeAsync(args);
        }
        finally
        {
            if (ClearAfterChange)
                await _fileDrop.TryInvokeVoidAsync("clear", Logger);
        }
    }
}
