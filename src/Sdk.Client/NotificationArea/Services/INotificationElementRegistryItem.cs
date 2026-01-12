using Microsoft.AspNetCore.Authorization;
using Sdk.Client.Modules;

namespace Sdk.Client.NotificationArea.Services;

/// <summary>
/// Item to register a notification element in a registry
/// </summary>
public interface INotificationElementRegistryItem
{
    Guid Id { get; }
    int Position { get; }
    Type ComponentType { get; }
    INotificationElementState State { get; }
    IAuthorizationRequirement? AuthorizationRequirement { get; }
}

/// <summary>
/// Item to register a notification element in a registry for a client module
/// </summary>
public interface INotificationElementRegistryItem<TClientModule> : INotificationElementRegistryItem
    where TClientModule : class, IClientModule;
