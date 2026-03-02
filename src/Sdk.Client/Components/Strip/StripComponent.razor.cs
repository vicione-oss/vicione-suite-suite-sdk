using Sdk.Client.Colors;
using ViciOne.Ui.MonochromeIcons.Core.Enums;

namespace Sdk.Client.Components.Strip;

/// <summary>
/// A component that displays a clickable, colored strip with an icon, headline, and subline.
/// </summary>
public partial class StripComponent
{
    /// <summary>
    /// Gets or sets the main headline text displayed on the strip.
    /// </summary>
    [Parameter] public string Headline { get; set; } = "";

    /// <summary>
    /// Gets or sets the secondary text or subline displayed below the headline.
    /// </summary>
    [Parameter] public string Subline { get; set; } = "";

    /// <summary>
    /// Gets or sets the background color of the strip.
    /// </summary>
    [Parameter] public IHtmlColor? Color { get; set; }

    /// <summary>
    /// Gets or sets the icon to be displayed on the strip.
    /// </summary>
    [Parameter] public MonochromeIconName IconName { get; set; }

    /// <summary>
    /// Gets or sets a callback that is invoked when the strip is clicked.
    /// </summary>
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// Gets or sets an additional CSS class to be applied to the component for custom styling.
    /// </summary>
    [Parameter] public string? CssClass { get; set; }

    private async Task OnClickAsync(MouseEventArgs e)
    {
        if (OnClick.HasDelegate)
            await OnClick.InvokeAsync(e);
    }
}
