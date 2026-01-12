using Microsoft.AspNetCore.Components;

namespace Sdk.Client.NotificationArea.Components;

/// <summary>
/// Default implementation for a number badge
/// </summary>
public abstract partial class NotificationElementNumberBadgeBase : ComponentBase, INotificationElementBadge
{
    /// <summary>
    /// Value displayed in the number badge
    /// </summary>
    /// <returns>Numeric value or null to hide the badge</returns>
    protected abstract int? GetNumber();
}
