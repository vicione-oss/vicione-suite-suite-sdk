namespace Sdk.Client.NotificationArea.Components;

/// <summary>
/// Base class of a badge that shows a number.
/// </summary>
public abstract partial class NotificationElementNumberBadgeBase : ComponentBase, INotificationElementBadge
{
    /// <summary>
    /// Returns the number to show.
    /// </summary>
    /// <returns>The number, or <see langword="null"/> to hide the badge.</returns>
    protected abstract int? GetNumber();
}
