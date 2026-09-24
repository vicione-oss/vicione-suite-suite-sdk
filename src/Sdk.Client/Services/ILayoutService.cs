using System.ComponentModel;

namespace Sdk.Client.Services;

/// <summary>
/// Defines a service for controlling the main application layout.
/// </summary>
public interface ILayoutService : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the preformatted title of the local suite instance.
    /// </summary>
    MarkupString InstanceTitle { get; }

    /// <summary>
    /// Gets the name of the local suite instance.
    /// </summary>
    string? InstanceName { get; }

    /// <summary>
    /// Gets or sets untranslated text shown in the title bar.
    /// </summary>
    string? TitleBarText { get; set; }

    /// <summary>
    /// Gets or sets the app name shown in the title bar.
    /// </summary>
    string? TitleBarAppName { get; set; }

    /// <summary>
    /// Gets or sets whether the page loading overlay is shown.
    /// </summary>
    bool IsLoadingOverlayVisible { get; set; }
}
