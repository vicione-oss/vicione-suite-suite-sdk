using Microsoft.AspNetCore.Authorization;
using Sdk.Client.Modules;

namespace Sdk.Client.NotificationArea.Services;

/// <summary>
/// Represents a registered notification element within a <see cref="INotificationElementRegistry"/>.
/// </summary>
public interface INotificationElementRegistryItem
{
    /// <summary>
    /// Gets the unique identifier of the notification element.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets the display position or order of the notification element within the notification area.
    /// </summary>
    int Position { get; }

    /// <summary>
    /// Gets the type of the Blazor component that renders the notification element.
    /// </summary>
    Type ComponentType { get; }

    /// <summary>
    /// Gets the state object that holds the dynamic data for the notification element.
    /// </summary>
    INotificationElementState State { get; }

    /// <summary>
    /// Gets the optional authorization requirement that must be met for the notification element to be visible.
    /// </summary>
    IAuthorizationRequirement? AuthorizationRequirement { get; }
}

/// <summary>
/// Represents a registered notificaton element within a <see cref="INotificationElementRegistry{TClientModule}"/>.
/// </summary>
public interface INotificationElementRegistryItem<TClientModule> : INotificationElementRegistryItem
    where TClientModule : class, IClientModule;
