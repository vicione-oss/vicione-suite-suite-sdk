namespace Sdk;

/// <summary>
/// Global SDK-wide constants for configuration sections, service keys, and predefined identifiers.
/// </summary>
public static class Constants
{
    /// <summary>
    /// The identifier for the core system module.
    /// </summary>
    public const string SystemModuleId = "System";

    /// <summary>
    /// The name of the configuration section for the module loader.
    /// </summary>
    public const string ModuleLoaderSection = "ModuleLoader";

    /// <summary>
    /// The name of the configuration section for the module API.
    /// </summary>
    public const string ModuleApiSection = "ModuleApi";

    /// <summary>
    /// The name of the configuration section for UI hosts, located within the <see cref="ModuleLoaderSection"/>.
    /// </summary>
    public const string UiHostsSection = ModuleLoaderSection + ":UiHosts";

    /// <summary>
    /// The prefix used to create authorization policy names for modules.
    /// </summary>
    public const string AuthorizationPolicyPrefix = "module";

    /// <summary>
    /// The DI service key for the client-side <c>TimeProvider</c>.
    /// </summary>
    public const string ClientTimeProviderServiceKey = "client-time-provider";
}
