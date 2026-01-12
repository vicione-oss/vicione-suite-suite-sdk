using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Sdk.Client.Extensions;
using Sdk.Client.Services;

namespace Sdk.Client.NavTiles.Components;

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

    [Parameter, EditorRequired] public string Headline { get; set; }
    [Parameter] public string? Subline { get; set; }
    [Parameter, EditorRequired] public string IconSrc { get; set; }
    [Parameter, EditorRequired] public string IconAlt { get; set; }
    [Parameter, EditorRequired] public EventCallback OnContentLoading { get; set; }
    [Parameter, EditorRequired] public EventCallback OnContentReady { get; set; }

    [Inject] private IJsInterop JsInterop { get; set; } = default!;

    [Inject] private ILogger<NavTileStandardContent> Logger { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        if (HasSubline() && string.CompareOrdinal(_previousSubline, Subline) != 0)
        {
            _previousSubline = Subline;

            await DisposeInitSublineResult();

            await OnContentLoading.InvokeAsync();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (HasSubline())
        {
            _jsModuleReference ??= await JsInterop.IncludeModuleScript("./_content/ViciOne.Suite.Sdk.Client/js/nav-tile-standard-content.js");

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

        _dotNetObjectReference?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeInitSublineResult();

        await _jsModuleReference.TryDisposeAsync(logger: Logger);
    }

    [JSInvokable]
    public async Task SublineInitialized() => await OnContentReady.InvokeAsync();

    private bool HasSubline() => !string.IsNullOrWhiteSpace(Subline);
}
