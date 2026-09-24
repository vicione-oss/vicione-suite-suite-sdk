namespace Sdk.Client.Components.Settings;

/// <summary>
/// How a <see cref="SettingsField"/> shows that it is loading.
/// </summary>
public enum SettingsFieldLoadingIndication
{
    /// <summary>
    /// A spinner animation.
    /// </summary>
    Spinner,

    /// <summary>
    /// Reduced opacity of the field's content.
    /// </summary>
    Opacity
}
