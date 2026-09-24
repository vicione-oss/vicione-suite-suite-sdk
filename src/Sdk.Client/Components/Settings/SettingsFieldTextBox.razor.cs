using ViciOne.Ui.Blazor.Components.TextBox;

namespace Sdk.Client.Components.Settings;

/// <summary>
/// A settings component that renders a text box for data entry.
/// </summary>
public sealed partial class SettingsFieldTextBox
{
    /// <inheritdoc cref="TextBox.Placeholder" />
    [Parameter]
    public string? Placeholder { get; set; }

    /// <inheritdoc cref="TextBox.Value" />
    [Parameter]
    public string? Value { get; set; }

    /// <inheritdoc cref="TextBox.ValueChanged" />
    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    /// <inheritdoc cref="TextBox.CssClass" />
    [Parameter]
    public string? CssClass { get; set; }

    /// <inheritdoc cref="TextBox.ReadOnly" />
    [Parameter]
    public bool ReadOnly { get; set; }

    /// <inheritdoc cref="TextBox.Enabled" />
    [Parameter]
    public bool Enabled { get; set; } = true;

    /// <inheritdoc cref="TextBox.Password" />
    [Parameter]
    public bool Password { get; set; }

    /// <summary>
    /// Gets or sets an optional text displayed below the input field.
    /// </summary>
    [Parameter]
    public string? Subline { get; set; }

    private async Task TextBoxValueChanged(string? value)
    {
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(value);
    }
}
