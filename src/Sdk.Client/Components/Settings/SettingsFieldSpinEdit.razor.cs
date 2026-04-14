using ViciOne.Ui.Blazor.Components.SpinEdit;

namespace Sdk.Client.Components.Settings;

/// <summary>
/// A settings component that renders a spin edit for numeric data entry.
/// </summary>
public sealed partial class SettingsFieldSpinEdit<TValue, TInterval, TLimit>
{
    /// <inheritdoc cref="SpinEdit{TValue, TInterval, TLimit}.Enabled" />
    [Parameter]
    public bool Enabled { get; set; } = true;

    /// <inheritdoc cref="SpinEdit{TValue, TInterval, TLimit}.ReadOnly" />
    [Parameter]
    public bool ReadOnly { get; set; }

    /// <inheritdoc cref="SpinEdit{TValue, TInterval, TLimit}.Value" />
    [Parameter]
    public TValue Value { get; set; } = default!;

    /// <inheritdoc cref="SpinEdit{TValue, TInterval, TLimit}.ValueChanged" />
    [Parameter]
    public EventCallback<TValue> ValueChanged { get; set; }

    /// <inheritdoc cref="SpinEdit{TValue, TInterval, TLimit}.Interval" />
    [Parameter, EditorRequired]
    public required TInterval Interval { get; set; }

    /// <inheritdoc cref="SpinEdit{TValue, TInterval, TLimit}.Minimum" />
    [Parameter, EditorRequired]
    public required TLimit Minimum { get; set; }

    /// <inheritdoc cref="SpinEdit{TValue, TInterval, TLimit}.Maximum" />
    [Parameter, EditorRequired]
    public required TLimit Maximum { get; set; }

    /// <inheritdoc cref="SpinEdit{TValue, TInterval, TLimit}.IsRastered" />
    [Parameter]
    public bool IsRastered { get; set; }

    /// <inheritdoc cref="SpinEdit{TValue, TInterval, TLimit}.Placeholder" />
    [Parameter]
    public string? Placeholder { get; set; }

    /// <inheritdoc cref="SpinEdit{TValue, TInterval, TLimit}.CssClass" />
    [Parameter]
    public string? CssClass { get; set; }

    private void SpinEditValueChanged(TValue value)
    {
        if (ValueChanged.HasDelegate)
            ValueChanged.InvokeAsync(value);
    }
}
