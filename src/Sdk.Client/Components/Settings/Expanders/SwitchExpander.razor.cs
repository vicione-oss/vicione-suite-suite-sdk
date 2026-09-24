using ViciOne.Ui.Blazor.Components.Switch;

namespace Sdk.Client.Components.Settings.Expanders;

/// <summary>
/// A settings expander component that renders a toggle switch.
/// </summary>
public sealed partial class SwitchExpander : ComponentBase
{
    /// <inheritdoc cref="Switch.Enabled"/>
    [Parameter]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets whether a loading indication should be rendered. When <see langword="true"/>, user input is blocked.
    /// </summary>
    [Parameter]
    public bool IsLoading { get; set; }

    /// <inheritdoc cref="Switch.Value"/>
    [Parameter]
    public bool Value { get; set; }

    /// <inheritdoc cref="Switch.ValueChanged"/>
    [Parameter]
    public EventCallback<bool> ValueChanged { get; set; }

    private async Task SwitchValueChanged(bool value)
    {
        if (IsLoading)
            return;

        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(value);
    }
}
