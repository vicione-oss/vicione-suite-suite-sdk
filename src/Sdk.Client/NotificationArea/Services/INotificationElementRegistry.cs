using Microsoft.AspNetCore.Authorization;
using Sdk.Client.Modules;
using Sdk.Client.NotificationArea.Components;
using Sdk.Client.Services;

namespace Sdk.Client.NotificationArea.Services;

/// <summary>
/// Registry for notification elements
/// </summary>
public interface INotificationElementRegistry : IRegistry<INotificationElementRegistryItem>;

/// <inheritdoc/>
public interface INotificationElementRegistry<TClientModule> : INotificationElementRegistry
    where TClientModule : class, IClientModule
{
    /// <summary>
    /// Registers a notification element in the registry.
    /// </summary>
    /// <param name="state">State for the notification element</param>
    /// <param name="position">Position of the notification element</param>
    /// <param name="id">Unique identifier for the notification element</param>
    INotificationElementRegistryItem Add<TElement, TState>(TState state, int? position = null, Guid? id = null, IAuthorizationRequirement? authorizationRequirement = null)
        where TElement : NotificationElementBase<TState>
        where TState : INotificationElementState;

    /// <summary>
    /// Removes an item from the registry based on the given unique identifier.
    /// </summary>
    /// <returns>True when item was removed</returns>
    bool Remove(Guid id);

    /// <summary>
    /// Removes all notification elements with the given type from the registry.
    /// </summary>
    /// <returns>True when item was removed</returns>
    int Remove<TNotificationElement>()
        where TNotificationElement : INotificationElement;
}
