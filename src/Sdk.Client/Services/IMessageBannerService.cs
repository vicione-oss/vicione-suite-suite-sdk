using Sdk.MessageBanner.Contracts;

namespace Sdk.Client.Services;

/// <summary>
/// Provides functionality to show and close a message banner in the user interface,
/// and notifies subscribers when the banner is closed.
/// </summary>
public interface IMessageBannerService
{
    /// <summary>
    /// Occurs when the currently displayed message banner is closed.
    /// </summary>
    /// <remarks>
    /// This event is raised after <see cref="CloseMessageBanner"/> is called
    /// or when the banner is otherwise dismissed by the system or user.
    /// </remarks>
    event Action? MessageBannerClosed;

    /// <summary>
    /// Displays a message banner with the specified type and description.
    /// </summary>
    void ShowMessageBanner(MessageType type, string? description);

    /// <summary>
    /// Closes the currently displayed message banner, if one is active.
    /// </summary>
    /// <remarks>
    /// After this method is called, the <see cref="MessageBannerClosed"/> event will be raised.
    /// Calling this method when no banner is visible is safe and has no effect.
    /// </remarks>
    void CloseMessageBanner();
}
