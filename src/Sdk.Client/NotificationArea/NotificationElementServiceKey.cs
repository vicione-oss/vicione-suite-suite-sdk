using Sdk.Client.Modules;
using Sdk.Client.NotificationArea.Components;

namespace Sdk.Client.NotificationArea;

/// <summary>
/// Service key of the services registered for one notification element of one module.
/// </summary>
public sealed class NotificationElementServiceKey<TClientModule, TNotificationElement>
    where TClientModule : class, IClientModule
    where TNotificationElement : ComponentBase, INotificationElement;
