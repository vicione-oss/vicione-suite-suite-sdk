using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Components.Settings;

public sealed partial class SettingsFieldCultureComboBox : ComponentBase
{
    private string _selectedCultureName = string.Empty;

    [Parameter]
    public bool Enabled { get; set; } = true;

    [Parameter, EditorRequired]
    public IEnumerable<CultureInfo> Items { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }

    [Parameter, EditorRequired]
    public CultureInfo Value { get; set; }

    [Parameter]
    public EventCallback<CultureInfo> ValueChanged { get; set; }

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
