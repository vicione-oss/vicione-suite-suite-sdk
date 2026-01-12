namespace Sdk.Modules;

/// <summary>
/// Defines the base contract for all modules in the ViciOne Suite SDK.
/// </summary>
public interface IModule
{
    /// <summary>
    /// Gets the unique key identifying this module.
    /// </summary>
    ModuleKey ModuleKey { get; }
}
