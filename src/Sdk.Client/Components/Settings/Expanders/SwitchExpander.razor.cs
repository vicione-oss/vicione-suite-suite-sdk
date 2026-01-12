using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Components.Settings.Expanders;

public sealed partial class SwitchExpander : ComponentBase
{
    [Parameter]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// <see langword="true"/> when a loading indication should be rendered and no input should be accepted, otherwise <see langword="false"/>
    /// </summary>
    [Parameter]
    public bool IsLoading { get; set; }

    [Parameter]
    public bool Value { get; set; }

    [Parameter]
    public EventCallback<bool> ValueChanged { get; set; }

    private async Task SwitchValueChanged(bool value)
    {
        if (IsLoading)
            return; // do not allow value change while loading is indicated

        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(value);
    }
}
