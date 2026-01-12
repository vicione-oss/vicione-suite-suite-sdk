using System.Diagnostics.CodeAnalysis;
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
    /// <typeparam name="TConnection">The connection type implementing <see cref="IConnection"/>.</typeparam>
    /// <typeparam name="TComponent">
    /// The Blazor component (or derived <see cref="ConnectionSettingsComponentBase{TConnection}"/>) that renders the UI for this connection type.
    /// </typeparam>
    /// <typeparam name="TValidator">
    /// The validator type (derived from <see cref="ItemValidatorBase{TConnection}"/>) used to validate this connection type.
    /// Must have a public parameterless constructor.
    /// </typeparam>
    /// <param name="id">The unique identifier for this connection type.</param>
    /// <param name="getDisplayName">
    /// A delegate that returns a display name for the connection type, suitable for showing in UI elements.
    /// </param>
    void Register<TConnection, TComponent, TValidator>(string id, Func<string> getDisplayName)
        where TConnection : IConnection
        where TComponent : ConnectionSettingsComponentBase<TConnection>
        where TValidator : ItemValidatorBase<TConnection>, new();

    /// <summary>
    /// Attempts to retrieve the display name for a given connection type identifier.
    /// </summary>
    /// <param name="id">The identifier of the connection type.</param>
    /// <param name="displayName">
    /// When this method returns <see langword="true"/>, contains the display name for the connection type.
    /// Otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the display name was found; otherwise, <see langword="false"/>.
    /// </returns>
    bool TryGetDisplayName(string id, [NotNullWhen(true)] out string? displayName);

    /// <summary>
    /// Attempts to retrieve the registered UI component type for a given connection type identifier.
    /// </summary>
    /// <param name="id">The identifier of the connection type.</param>
    /// <param name="componentType">
    /// When this method returns <see langword="true"/>, contains the component <see cref="Type"/> for the connection type.
    /// Otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the component type was found; otherwise, <see langword="false"/>.
    /// </returns>
    bool TryGetComponentType(string id, [NotNullWhen(true)] out Type? componentType);

    /// <summary>
    /// Attempts to retrieve the registered validator instance for a given connection type identifier.
    /// </summary>
    /// <param name="id">The identifier of the connection type.</param>
    /// <param name="itemValidator">
    /// When this method returns <see langword="true"/>, contains the <see cref="IItemValidator"/> instance for the connection type.
    /// Otherwise, <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the validator was found; otherwise, <see langword="false"/>.
    /// </returns>
    bool TryGetItemValidator(string id, [NotNullWhen(true)] out IItemValidator? itemValidator);
}

