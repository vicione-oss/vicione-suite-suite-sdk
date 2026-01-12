namespace Sdk.Client.NotificationArea.Services;

/// <summary>
/// Policy for active notification elements
/// </summary>
public interface IActiveNotificationElementPolicy
{
    /// <summary>
    /// Includes a notification element in the policy
    /// </summary>
    void Include(INotificationElementState notificationElementState);

    /// <summary>
    /// Excludes a notification element from the policy
    /// </summary>
    void Exclude(INotificationElementState notificationElementState);

    /// <summary>
    /// Ensures that all notification elements included in the policy are not active
    /// </summary>
    void NoneActive();
}
