using System.ComponentModel;

namespace Sdk.Client.Services;

/// <summary>
/// Defines a service for controlling the main application layout.
/// </summary>
public interface ILayoutService : INotifyPropertyChanged
{
    /// <summary>
    /// Preformatted title of the local suite instance
    /// </summary>
    MarkupString InstanceTitle { get; }

    /// <summary>
    /// Name of the local suite instance
    /// </summary>
    string? InstanceName { get; }

    /// <summary>
    /// Use to set untranslated text on top bar
    /// </summary>
    string? TitleBarText { get; set; }

    /// <summary>
    /// Use to set the app name
    /// </summary>
    string? TitleBarAppName { get; set; }

    /// <summary>
    /// Use to toggle the loading overlay for pages
    /// </summary>
    bool IsLoadingOverlayVisible { get; set; }
}
