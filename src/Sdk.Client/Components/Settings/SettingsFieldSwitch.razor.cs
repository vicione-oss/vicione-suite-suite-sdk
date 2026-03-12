using ViciOne.Ui.Blazor.Components.Switch;
using ViciOne.Ui.Blazor.Components.Switch.Enums;

namespace Sdk.Client.Components.Settings;

/// <summary>
/// A settings component that renders a switch for data entry.
/// </summary>
public sealed partial class SettingsFieldSwitch
{
    /// <inheritdoc cref="Switch.Enabled" />
    [Parameter]
    public bool Enabled { get; set; }

    /// <inheritdoc cref="Switch.Value" />
    [Parameter]
    public bool Value { get; set; }

    /// <inheritdoc cref="Switch.ValueChanged" />
    [Parameter]
    public EventCallback<bool> ValueChanged { get; set; }

    /// <inheritdoc cref="Switch.Size" />
    [Parameter]
    public SwitchSize Size { get; set; } = SwitchSize.Medium;

    /// <inheritdoc cref="Switch.CssClass" />
    [Parameter]
    public string? CssClass { get; set; }

    private void SwitchValueChanged(bool value)
    {
        if (ValueChanged.HasDelegate)
            ValueChanged.InvokeAsync(value);
    }
}
