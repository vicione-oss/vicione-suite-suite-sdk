using ViciOne.Ui.MonochromeIcons.Core.Enums;

namespace Sdk.Client.Components.LinkButton;

/// <summary>
/// A component that renders a button styled as a hyperlink, typically with an icon and text.
/// </summary>
public sealed partial class LinkButton
{
    /// <summary>
    /// Gets or sets the icon to be displayed on the button.
    /// </summary>
    [Parameter, EditorRequired]
    public MonochromeIconName IconName { get; set; }

    /// <summary>
    /// Gets or sets an additional CSS class to be applied to the component for custom styling.
    /// </summary>
    [Parameter]
    public string? CssClass { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the button is enabled and allows user interaction.
    /// </summary>
    [Parameter]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the unique identifier for the button element.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets a callback that is invoked when the button is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// Gets or sets the tooltip text for the button, which is rendered into the 'title' attribute.
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the text displayed on the button.
    /// </summary>
    [Parameter]
    public string? Text { get; set; }

    private async Task ButtonClickAsync(MouseEventArgs e)
    {
        if (OnClick.HasDelegate)
            await OnClick.InvokeAsync(e);
    }
}
