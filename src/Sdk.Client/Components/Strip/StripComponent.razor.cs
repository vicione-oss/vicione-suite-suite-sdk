using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Sdk.Client.Colors;
using ViciOne.Ui.MonochromeIcons.Core.Enums;

namespace Sdk.Client.Components.Strip;

public partial class StripComponent
{
    [Parameter] public string Headline { get; set; } = "";
    [Parameter] public string Subline { get; set; } = "";
    [Parameter] public IHtmlColor? Color { get; set; }
    [Parameter] public MonochromeIconName IconName { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    [Parameter] public string? CssClass { get; set; }

    private async Task OnClickAsync(MouseEventArgs e)
    {
        if (OnClick.HasDelegate)
            await OnClick.InvokeAsync(e);
    }
}
