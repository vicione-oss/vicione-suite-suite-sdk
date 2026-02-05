using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Sdk.Client.Extensions;
using Sdk.Client.Services;

namespace Sdk.Client.NavTiles.Components;

/// <summary>
/// Represents the standard content layout for a navigation tile, including a headline, an optional subline, and an icon.
/// </summary>
public sealed partial class NavTileStandardContent : ComponentBase, IAsyncDisposable
{
    private ElementReference? _contentElementReference;
    private ElementReference? _headlineElementReference;
    private ElementReference? _sublineElementReference;
    private ElementReference? _iconElementReference;
    private DotNetObjectReference<NavTileStandardContent>? _dotNetObjectReference;
    private IJSObjectReference? _jsModuleReference;
    private IJSObjectReference? _initSublineResult;
    private string? _previousSubline;
    private bool _sublineInitialized;

    /// <summary>
    /// Gets or sets the main headline text for the tile.
    /// </summary>
    [Parameter, EditorRequired] public string Headline { get; set; }

    /// <summary>
    /// Gets or sets the optional subline text for the tile.
    /// </summary>
    [Parameter] public string? Subline { get; set; }

    /// <summary>
    /// Gets or sets the source URL for the icon image.
    /// </summary>
    [Parameter, EditorRequired] public string IconSrc { get; set; }

    /// <summary>
    /// Gets or sets the alternative text for the icon image, used for accessibility.
    /// </summary>
    [Parameter, EditorRequired] public string IconAlt { get; set; }

    /// <summary>
    /// Gets or sets a callback that is invoked when the content, particularly the subline, begins its initialization process.
    /// </summary>
    [Parameter, EditorRequired] public EventCallback OnContentLoading { get; set; }

    /// <summary>
    /// Gets or sets a callback that is invoked when the content has finished its initialization and is fully rendered.
    /// </summary>
    [Parameter, EditorRequired] public EventCallback OnContentReady { get; set; }

    [Inject] private IJsInterop JsInterop { get; set; } = default!;

    [Inject] private ILogger<NavTileStandardContent> Logger { get; set; } = default!;

    /// <inheritdoc/>
    protected override async Task OnParametersSetAsync()
    {
        if (HasSubline() && string.CompareOrdinal(_previousSubline, Subline) != 0)
        {
            _previousSubline = Subline;

            await DisposeInitSublineResult();

            _sublineInitialized = false;

            await OnContentLoading.InvokeAsync();
        }
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (HasSubline())
        {
            _jsModuleReference ??= await JsInterop.IncludeModuleScript(new Uri("./_content/ViciOne.Suite.Sdk.Client/js/nav-tile-standard-content.js", UriKind.Relative));

            if (_initSublineResult is null)
            {
                if (_sublineElementReference is not null &&
                    _contentElementReference is not null &&
                    _headlineElementReference is not null &&
                    _iconElementReference is not null &&
                    _jsModuleReference is not null)
                {
                    // no invoke of OnContentLoading here as the element is already displayed in the browser, moved to OnParametersSetAsync()
                    _dotNetObjectReference = DotNetObjectReference.Create(this);

                    _initSublineResult = await _jsModuleReference.InvokeAsync<IJSObjectReference>("init", _sublineElementReference, _headlineElementReference, _contentElementReference, _iconElementReference, _dotNetObjectReference);
                }
            }
        }
        else
        {
            await DisposeInitSublineResult();
        }
    }

    private async Task DisposeInitSublineResult()
    {
        await _initSublineResult.TryInvokeVoidAsync("dispose", Logger);
        await _initSublineResult.TryDisposeAsync(Logger);
        _initSublineResult = null;

        _dotNetObjectReference?.Dispose();
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await DisposeInitSublineResult();
        await OnContentReady.InvokeAsync();

        await _jsModuleReference.TryDisposeAsync(logger: Logger);
    }

    /// <summary>
    /// A method invoked by JavaScript to signal that the subline initialization is complete.
    /// </summary>
    [JSInvokable]
    public async Task SublineInitialized()
    {
        _sublineInitialized = true;
        await InvokeAsync(StateHasChanged);

        await OnContentReady.InvokeAsync();
    }

    private bool HasSubline() => !string.IsNullOrWhiteSpace(Subline);
}
