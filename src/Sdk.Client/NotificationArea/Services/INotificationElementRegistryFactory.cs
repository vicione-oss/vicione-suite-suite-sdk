using Sdk.Client.Modules;

namespace Sdk.Client.NotificationArea.Services;

/// <summary>
/// Defines a factory for creating notification element registries.
/// </summary>
/// <remarks>
/// This interface is intended for internal framework use only and should not be used directly by consumer applications.
/// </remarks>
public interface INotificationElementRegistryFactory
{
    /// <summary>
    /// Creates a new notification element registry for a specific client module.
    /// </summary>
    INotificationElementRegistry<TClientModule> CreateNotificationElementRegistry<TClientModule>() where TClientModule : class, IClientModule;
}
