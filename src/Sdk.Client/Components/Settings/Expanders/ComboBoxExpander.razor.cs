using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;

namespace Sdk.Client.Components.Settings.Expanders;

public sealed partial class ComboBoxExpander<TItem, TValue> : ComponentBase
{
    [Parameter]
    public bool Enabled { get; set; } = true;

    [Parameter, EditorRequired]
    public IEnumerable<TItem> Items { get; set; }

    [Parameter, EditorRequired]
    public TValue Value { get; set; }

    [Parameter]
    public EventCallback<TValue> ValueChanged { get; set; }

    [Parameter]
    public Expression<Func<TItem, TValue>>? ValueSelector { get; set; }

    [Parameter]
    public Expression<Func<TItem, string>>? TextSelector { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }

    private async Task ComboBoxValueChanged(TValue value)
    {
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(value);
    }
}
