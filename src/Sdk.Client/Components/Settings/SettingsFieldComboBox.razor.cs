using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Components.Settings;

[CascadingTypeParameter(nameof(TItem))]
[CascadingTypeParameter(nameof(TValue))]
public sealed partial class SettingsFieldComboBox<TItem, TValue> : ComponentBase
{
    [Parameter]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Items selectable from the drop-down of the combo-box
    /// </summary>
    [Parameter, EditorRequired]
    public IEnumerable<TItem> Items { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }

    /// <summary>
    /// <see cref="ValueSelector">Value</see> of the selected <typeparamref name="TItem"/>
    /// </summary>
    [Parameter, EditorRequired]
    public TValue Value { get; set; }

    /// <summary>
    /// Raised when <see cref="Value" /> has changed
    /// </summary>
    [Parameter]
    public EventCallback<TValue> ValueChanged { get; set; }

    /// <summary>
    /// Selector for the value property of <typeparamref name="TItem"/>
    /// </summary>
    [Parameter]
    public Expression<Func<TItem, TValue>>? ValueSelector { get; set; }

    /// <summary>
    /// Selector for the text property of <typeparamref name="TItem"/>
    /// </summary>
    [Parameter]
    public Expression<Func<TItem, string>>? TextSelector { get; set; }

    private async Task ComboBoxValueChanged(TValue value)
    {
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(value);
    }
}
