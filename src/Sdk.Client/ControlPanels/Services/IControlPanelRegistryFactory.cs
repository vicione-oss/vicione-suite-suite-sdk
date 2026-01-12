using Sdk.Client.Modules;

namespace Sdk.Client.ControlPanels.Services;

/// <summary>
/// Defines a factory for creating control panel registries.
/// </summary>
/// <remarks>
/// This interface is intended for internal framework use only and should not be used directly by consumer applications.
/// </remarks>
public interface IControlPanelRegistryFactory
{
    /// <summary>
    /// Creates a new control panel registry associated with <typeparamref name="TClientModule"/>.
    /// </summary>
    IControlPanelRegistry<TClientModule> CreateControlPanelRegistry<TClientModule>() where TClientModule : class, IClientModule;
}
