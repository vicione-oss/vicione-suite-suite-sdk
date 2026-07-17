using Microsoft.AspNetCore.Components;
using ViciOne.Ui.MonochromeIcons.Assets.Services;
using ViciOne.Ui.MonochromeIcons.Core.Enums;

namespace Sdk.Client.Samples.Shared.Components;

public sealed partial class NavigationItem : ComponentBase
{
    private MarkupString _iconSvgMarkup;

    [Parameter, EditorRequired] public string Text { get; set; }
    [Parameter] public string? Href { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Inject] private IMonochromeIconSvgMarkupProvider MonochromeIconSvgMarkupProvider { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var iconSvgMarkup = await MonochromeIconSvgMarkupProvider.GetSvgMarkupAsync(MonochromeIconName.ExpanderLightRight,
            MonochromeIconSize.Medium);

        _iconSvgMarkup = new MarkupString(iconSvgMarkup ?? string.Empty);
    }
}
