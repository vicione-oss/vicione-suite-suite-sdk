using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ViciOne.Ui.MonochromeIcons.Core.Enums;

namespace Sdk.Client.Components.LinkButton;

public sealed partial class LinkButton
{
    [Parameter, EditorRequired]
    public MonochromeIconName IconName { get; set; }

    /// <summary>
    /// Text rendered into <see href="https://html.spec.whatwg.org/#classes">class</see> attribute
    /// </summary>
    [Parameter]
    public string? CssClass { get; set; }

    /// <summary>
    /// True when user interaction should be allowed, otherwise false
    /// </summary>
    [Parameter]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Text rendered into <see href="https://html.spec.whatwg.org/#the-id-attribute">id</see> attribute
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Raised when button has been clicked
    /// </summary>
    [Parameter]
    public EventCallback OnClick { get; set; }

    /// <summary>
    /// Text rendered into <see href="https://html.spec.whatwg.org/#attr-title">title</see> attribute
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Text displayed in the button
    /// </summary>
    [Parameter]
    public string? Text { get; set; }

    private async Task ButtonClickAsync(MouseEventArgs e)
    {
        if (OnClick.HasDelegate)
            await OnClick.InvokeAsync(e);
    }
}
