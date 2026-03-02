namespace Sdk.Client.Components.Settings;

/// <summary>
/// A reusable component that renders plus and minus buttons, commonly used for implementing stepper controls.
/// </summary>
public sealed partial class SettingsStepper : ComponentBase
{
    /// <summary>
    /// Gets or sets a value indicating whether the plus (increment) button is enabled.
    /// </summary>
    [Parameter]
    public bool PlusEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the minus (decrement) button is enabled.
    /// </summary>
    [Parameter]
    public bool MinusEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a callback that is invoked when the plus button is clicked.
    /// </summary>
    [Parameter]
    public EventCallback OnPlus { get; set; }

    /// <summary>
    /// Gets or sets a callback that is invoked when the minus button is clicked.
    /// </summary>
    [Parameter]
    public EventCallback OnMinus { get; set; }

    /// <summary>
    /// Gets or sets a callback that is invoked when either the plus or the minus button is clicked.
    /// This is fired after the specific <see cref="OnPlus"/> or <see cref="OnMinus"/> event.
    /// </summary>
    [Parameter]
    public EventCallback OnPlusMinus { get; set; }

    private async Task PlusButtonClick()
    {
        if (OnPlus.HasDelegate)
            await OnPlus.InvokeAsync();

        if (OnPlusMinus.HasDelegate)
            await OnPlusMinus.InvokeAsync();
    }

    private async Task MinusButtonClick()
    {
        if (OnMinus.HasDelegate)
            await OnMinus.InvokeAsync();

        if (OnPlusMinus.HasDelegate)
            await OnPlusMinus.InvokeAsync();
    }
}
