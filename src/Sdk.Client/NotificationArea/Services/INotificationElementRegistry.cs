using Microsoft.AspNetCore.Authorization;
using Sdk.Client.Modules;
using Sdk.Client.NotificationArea.Components;
using Sdk.Client.Services;

namespace Sdk.Client.NotificationArea.Services;

/// <summary>
/// Holds a module's notification elements; this non-generic view lets the host enumerate every module's registry.
/// </summary>
public interface INotificationElementRegistry : IRegistry<INotificationElementRegistryItem>;

/// <inheritdoc/>
public interface INotificationElementRegistry<TClientModule> : INotificationElementRegistry
    where TClientModule : class, IClientModule
{
    /// <summary>
    /// Registers a notification element in the registry.
    /// </summary>
    /// <param name="state">The element's state.</param>
    /// <param name="position">The element's position in the notification area; <see langword="null"/> uses the default.</param>
    /// <param name="id">Identifies the element; <see langword="null"/> generates one.</param>
    /// <param name="authorizationRequirement">The requirement to see the element; <see langword="null"/> skips authorization.</param>
    INotificationElementRegistryItem Add<TElement, TState>(TState state, int? position = null, Guid? id = null, IAuthorizationRequirement? authorizationRequirement = null)
        where TElement : NotificationElementBase<TState>
        where TState : INotificationElementState;

    /// <summary>
    /// Removes the notification element with the given ID.
    /// </summary>
    /// <returns><see langword="true"/> if an element was removed.</returns>
    bool Remove(Guid id);

    /// <summary>
    /// Removes all notification elements of type <typeparamref name="TNotificationElement"/>.
    /// </summary>
    /// <returns>The number of elements removed.</returns>
    int Remove<TNotificationElement>()
        where TNotificationElement : INotificationElement;
}
