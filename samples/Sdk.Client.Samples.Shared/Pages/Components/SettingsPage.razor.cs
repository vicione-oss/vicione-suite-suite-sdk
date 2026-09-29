using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Sdk.Client.Components.Settings;
using Sdk.Client.Samples.Shared.Models;
using Sdk.Client.Samples.Shared.Services;
using ViciOne.Ui.Blazor.Components.ComboBox;
namespace Sdk.Client.Samples.Shared.Pages.Components;

public sealed partial class SettingsPage : ComponentBase
{
    private static readonly LogLevel[] s_logLevelComboBoxItems = Enum.GetValues<LogLevel>();
    private static readonly LogLevel[] s_logLevelComboBoxExpanderItems = Enum.GetValues<LogLevel>();
    private static readonly CultureInfo[] s_cultureComboBoxItems = [new("en-US"), new("de-DE")];

    private SettingsFieldFileUpload<TestUploadTicket>? _fileUpload;

    // The upload field cancels a running upload whenever its filename, ticket factory or handler parameter changes. So
    // the factory and handler live in fields: created in the markup, every render of this page would pass new instances.
    // The filename is bound: the field reports the picked file through FilenameChanged, and when the value comes back on
    // the next render it matches the running upload, so that upload continues.
    private readonly TestUploadTicketFactory _uploadTicketFactory = new();
    private readonly TestUploadHandler _uploadHandler = new();
    private string? _uploadFilename;

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
    private LogLevel _logLevelComboBoxExpander = LogLevel.None;
    private string _firstName = "";
    private bool _enabled;
    private CultureInfo _culture = s_cultureComboBoxItems[0];

    private bool SettingsFieldIsLoading => _selectedSettingsFieldLoadingIndication is not null;
    private SettingsFieldLoadingIndication SettingsFieldLoadingIndication => _selectedSettingsFieldLoadingIndication ?? default;

    public async Task ResetFileUpload()
    {
        if (_fileUpload is null)
        {
            return;
        }

        await _fileUpload.ResetAsync();
    }

    // Shows the pattern for a setting that takes time to apply: set IsLoading while saving, so the switch blocks further
    // input, and clear it afterwards. The delay stands in for a call to the backend.
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
