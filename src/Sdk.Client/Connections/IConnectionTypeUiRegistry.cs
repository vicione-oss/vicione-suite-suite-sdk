using Sdk.Connections.Contracts;

namespace Sdk.Client.Connections;

/// <summary>
/// Provides a registry for associating connection types with their corresponding UI components,
/// display names, and validators.
/// </summary>
/// <remarks>
/// This registry is typically used by configuration UIs to look up the correct settings component,
/// display name, and validator for a given connection type identifier.
/// </remarks>
public interface IConnectionTypeUiRegistry
{
    /// <summary>
    /// Registers a connection type with its associated UI component and validator.
    /// </summary>
    void Register<TConnection, TComponent, TValidator>(string id, Func<string> getDisplayName)
        where TConnection : IConnection
        where TComponent : ConnectionSettingsComponentBase<TConnection>
        where TValidator : ItemValidatorBase<TConnection>, new();

    /// <summary>
    /// Attempts to retrieve the display name for a given connection type identifier.
    /// </summary>
    bool TryGetDisplayName(string id, [NotNullWhen(true)] out string? displayName);

    /// <summary>
    /// Attempts to retrieve the registered UI component type for a given connection type identifier.
    /// </summary>
    bool TryGetComponentType(string id, [NotNullWhen(true)] out Type? componentType);

    /// <summary>
    /// Attempts to retrieve the registered validator instance for a given connection type identifier.
    /// </summary>
    bool TryGetItemValidator(string id, [NotNullWhen(true)] out IItemValidator? itemValidator);
}

