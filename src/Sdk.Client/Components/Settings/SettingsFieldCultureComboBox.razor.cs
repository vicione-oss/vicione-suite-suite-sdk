using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Components.Settings;

/// <summary>
/// A settings component that renders a combo box for selecting a <see cref="CultureInfo"/>.
/// </summary>
public sealed partial class SettingsFieldCultureComboBox : ComponentBase
{
    private string _selectedCultureName = string.Empty;

    /// <inheritdoc cref="SettingsFieldComboBox{CultureInfo, String}.Enabled"/>
    [Parameter]
    public bool Enabled { get; set; } = true;

    /// <inheritdoc cref="SettingsFieldComboBox{CultureInfo, String}.Items"/>
    [Parameter, EditorRequired]
    public IEnumerable<CultureInfo> Items { get; set; }

    /// <inheritdoc cref="SettingsFieldComboBox{CultureInfo, String}.ReadOnly"/>
    [Parameter]
    public bool ReadOnly { get; set; }

    /// <inheritdoc cref="SettingsFieldComboBox{CultureInfo, String}.Value"/>
    [Parameter, EditorRequired]
    public CultureInfo Value { get; set; }

    /// <inheritdoc cref="SettingsFieldComboBox{CultureInfo, String}.ValueChanged"/>
    [Parameter]
    public EventCallback<CultureInfo> ValueChanged { get; set; }

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        _selectedCultureName = Value.Name;
    }

    private async Task ComboBoxValueChanged()
    {
        if (ValueChanged.HasDelegate)
        {
            var selectedCulture = Items.FirstOrDefault(i => i.Name.Equals(_selectedCultureName, StringComparison.OrdinalIgnoreCase));

            if (selectedCulture is not null)
                await ValueChanged.InvokeAsync(selectedCulture);
        }
    }
}
