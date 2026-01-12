namespace Sdk.Client.NotificationArea.Services;

/// <summary>
/// State for a notification element
/// </summary>
public interface INotificationElementState
{
    /// <summary>
    /// Returns true when the notification element is active.
    /// 
    /// This flag can be used to indicate an active state, for example after the element has been clicked.
    /// 
    /// The default value is false. <see cref="Visible"/> must be true when setting this proprety to true, otherwise the assignment is discarded.
    /// </summary>
    bool IsActive { get; set; }

    /// <summary>
    /// Returns true when the notification element should be visible.
    /// 
    /// The default value is true. When set to false then <see cref="IsActive"/> is also set to false.
    /// </summary>
    bool Visible { get; set; }

    /// <summary>
    /// Raised when state has been changed and no <see cref="BeginUpdate">update cycle</see> is running.
    /// </summary>
    event Action<NotificationElementStateChangedEventArgs>? Changed;

    /// <summary>
    /// Call this method to begin an update cycle.
    /// 
    /// Make sure to have a corresponding call to <see cref="EndUpdate"/> to end the update cycle.
    /// </summary>
    void BeginUpdate();

    /// <summary>
    /// Call this method to end an update cycle.
    /// </summary>
    void EndUpdate();
}
