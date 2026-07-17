using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Sdk.Client.Components.Settings;
using ViciOne.Ui.Blazor.Components.ComboBox;
namespace Sdk.Client.Samples.Shared.Pages.Components;

public sealed partial class SettingsPage : ComponentBase
{
    private static readonly LogLevel[] s_logLevelComboBoxItems = Enum.GetValues<LogLevel>();
    private static readonly CultureInfo[] s_cultureComboBoxItems = [new("en-US"), new("de-DE")];
    private bool _switchExpanderIsLoading;
    private bool _switchExpanderValue;
    private int _width = 320;
    private bool _settingsFieldLoadingIndicationExamplesExpanded = true;
    private readonly List<ComboBoxItem<SettingsFieldLoadingIndication?, string>> _settingsFieldLoadingIndications =
        [
            new () {Text = "<None>", Value = null},
            .. Enum.GetValues<SettingsFieldLoadingIndication>().Select(loadingIndication => new ComboBoxItem<SettingsFieldLoadingIndication?, string>
            {
                Value = loadingIndication,
                Text = loadingIndication.ToString()
            })
            .OrderBy(i => i.Text)
        ];
    private SettingsFieldLoadingIndication? _selectedSettingsFieldLoadingIndication;
    private LogLevel _logLevel = LogLevel.None;
    private string _firstName = "";
    private bool _enabled;
    private CultureInfo _culture = s_cultureComboBoxItems[0];

    private bool SettingsFieldIsLoading => _selectedSettingsFieldLoadingIndication is not null;
    private SettingsFieldLoadingIndication SettingsFieldLoadingIndication => _selectedSettingsFieldLoadingIndication ?? default;
    [Inject]
    public static ILogger<SettingsPage> Logger { get; set; } = default!;

    private async Task SwitchExpanderValueChanged(bool value)
    {
        _switchExpanderValue = value;
        _switchExpanderIsLoading = true;
        await InvokeAsync(StateHasChanged);

        await Task.Delay(3000);

        _switchExpanderIsLoading = false;
        await InvokeAsync(StateHasChanged);
    }
}
