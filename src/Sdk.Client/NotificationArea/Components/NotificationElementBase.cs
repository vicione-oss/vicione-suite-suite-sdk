using Microsoft.AspNetCore.Components;
using Sdk.Client.NotificationArea.Services;

namespace Sdk.Client.NotificationArea.Components;

/// <summary>
/// Base class for notification elements
/// </summary>
/// <typeparam name="TState">Class implementing the state of the notification element</typeparam>
public class NotificationElementBase<TState> : ComponentBase, INotificationElement
    where TState : INotificationElementState
{
    [Parameter, EditorRequired]
    public TState State { get; set; }
}
