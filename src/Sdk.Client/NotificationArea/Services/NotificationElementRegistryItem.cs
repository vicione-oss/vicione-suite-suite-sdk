using Microsoft.AspNetCore.Authorization;
using Sdk.Client.Modules;

namespace Sdk.Client.NotificationArea.Services;

internal sealed class NotificationElementRegistryItem<TClientModule>(Guid id, int position, Type componentType, INotificationElementState state, IAuthorizationRequirement? authorizationRequirement) :
    INotificationElementRegistryItem<TClientModule>
    where TClientModule : class, IClientModule
{
    public Guid Id { get; set; } = id;
    public int Position { get; } = position;
    public Type ComponentType { get; } = componentType;
    public INotificationElementState State { get; } = state;

    /// <summary>
    /// Optional authorization requirement, otherwise <see langword="null" /> to skip authorization
    /// </summary>
    public IAuthorizationRequirement? AuthorizationRequirement { get; } = authorizationRequirement;
}
