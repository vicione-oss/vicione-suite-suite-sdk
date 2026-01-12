using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Components.Settings;

/// <summary>
/// Renders a plus and minus button to allow implementing a stepper-like behavior in a unified way
/// </summary>
public sealed partial class SettingsStepper : ComponentBase
{
    [Parameter]
    public bool PlusEnabled { get; set; } = true;

    [Parameter]
    public bool MinusEnabled { get; set; } = true;

    [Parameter]
    public EventCallback OnPlus { get; set; }

    [Parameter]
    public EventCallback OnMinus { get; set; }

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
