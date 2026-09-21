using ViciOne.Ui.Blazor.Components.Button;
using ViciOne.Ui.Blazor.Components.Button.Enums;

namespace Sdk.Client.Components.Settings;

/// <summary>
/// A settings component that renders a button.
/// </summary>
public sealed partial class SettingsFieldButton : ComponentBase
{
    /// <inheritdoc cref="Button.Busy"/>
    [Parameter]
    public bool Busy { get; set; }

    /// <inheritdoc cref="Button.BusyIndication"/>
    [Parameter]
    public ButtonBusyIndication BusyIndication { get; set; } = ButtonBusyIndication.Sweep;

    /// <inheritdoc cref="Button.CssClass"/>
    [Parameter]
    public string? CssClass { get; set; }

    /// <inheritdoc cref="Button.Enabled"/>
    [Parameter]
    public bool Enabled { get; set; } = true;

    /// <inheritdoc cref="Button.OnClick"/>
    [Parameter]
    public EventCallback OnClick { get; set; }

    /// <inheritdoc cref="Button.Title"/>
    [Parameter]
    public string? Title { get; set; }

    /// <inheritdoc cref="Button.IconCssClass"/>
    [Parameter]
    public string? IconCssClass { get; set; }

    /// <inheritdoc cref="Button.IconUrl"/>
    [Parameter]
    public Uri? IconUrl { get; set; }

    /// <inheritdoc cref="Button.Text"/>
    [Parameter]
    public string? Text { get; set; }
}
