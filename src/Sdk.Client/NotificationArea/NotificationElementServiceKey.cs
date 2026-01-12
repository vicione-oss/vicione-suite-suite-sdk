using Microsoft.AspNetCore.Components;
using Sdk.Client.Modules;
using Sdk.Client.NotificationArea.Components;

namespace Sdk.Client.NotificationArea;

/// <summary>
/// Class type that is used as service key in notification element service registrations.
/// </summary>
public sealed class NotificationElementServiceKey<TClientModule, TNotificationElement>
    where TClientModule : class, IClientModule
    where TNotificationElement : ComponentBase, INotificationElement;
