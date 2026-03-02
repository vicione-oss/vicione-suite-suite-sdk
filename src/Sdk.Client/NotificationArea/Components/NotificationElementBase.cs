using Sdk.Client.NotificationArea.Services;

namespace Sdk.Client.NotificationArea.Components;

/// <summary>
/// Base class for a notification element.
/// </summary>
/// <typeparam name="TState">The type of the state object for the notification element.</typeparam>
public class NotificationElementBase<TState> : ComponentBase, INotificationElement
    where TState : INotificationElementState
{
    /// <summary>
    /// Gets or sets the state object that holds the dynamic data for the notification element.
    /// </summary>
    [Parameter, EditorRequired]
    public TState State { get; set; }
}
