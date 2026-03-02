using System.Linq.Expressions;
using ViciOne.Ui.Blazor.Components.ComboBox;

namespace Sdk.Client.Components.Settings.Expanders;

/// <summary>
/// A settings expander component that renders a combo box for selecting a value from a list.
/// </summary>
public sealed partial class ComboBoxExpander<TItem, TValue> : ComponentBase
{
    /// <inheritdoc cref="ComboBox{TItem, TValue}.Enabled"/>
    [Parameter]
    public bool Enabled { get; set; } = true;

    /// <inheritdoc cref="ComboBox{TItem, TValue}.Items"/>
    [Parameter, EditorRequired]
    public IEnumerable<TItem> Items { get; set; }

    /// <inheritdoc cref="ComboBox{TItem, TValue}.Value"/>
    [Parameter, EditorRequired]
    public TValue Value { get; set; }

    /// <inheritdoc cref="ComboBox{TItem, TValue}.ValueChanged"/>
    [Parameter]
    public EventCallback<TValue> ValueChanged { get; set; }

    /// <inheritdoc cref="ComboBox{TItem, TValue}.ValueSelector"/>
    [Parameter]
    public Expression<Func<TItem, TValue>>? ValueSelector { get; set; }

    /// <inheritdoc cref="ComboBox{TItem, TValue}.TextSelector"/>
    [Parameter]
    public Expression<Func<TItem, string>>? TextSelector { get; set; }

    /// <inheritdoc cref="ComboBox{TItem, TValue}.ReadOnly"/>
    [Parameter]
    public bool ReadOnly { get; set; }

    private async Task ComboBoxValueChanged(TValue value)
    {
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(value);
    }
}
