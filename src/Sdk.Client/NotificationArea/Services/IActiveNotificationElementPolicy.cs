namespace Sdk.Client.NotificationArea.Services;

/// <summary>
/// Tracks notification elements whose active state is managed together.
/// </summary>
public interface IActiveNotificationElementPolicy
{
    /// <summary>
    /// Adds a notification element to the policy.
    /// </summary>
    void Include(INotificationElementState notificationElementState);

    /// <summary>
    /// Removes a notification element from the policy.
    /// </summary>
    void Exclude(INotificationElementState notificationElementState);

    /// <summary>
    /// Deactivates every notification element in the policy.
    /// </summary>
    void NoneActive();
}
