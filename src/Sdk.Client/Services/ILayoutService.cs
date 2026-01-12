using System.ComponentModel;
using Microsoft.AspNetCore.Components;

namespace Sdk.Client.Services;

public interface ILayoutService : INotifyPropertyChanged
{
    /// <summary>
    /// Preformatted title of the local suite instance
    /// </summary>
    /// <remarks>
    /// This property is configurable in settings dialog.
    /// </remarks>
    MarkupString InstanceTitle { get; }

    /// <summary>
    /// Name of the local suite instance
    /// </summary>
    /// <remarks>
    /// This property is configurable in settings dialog.
    /// </remarks>
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
