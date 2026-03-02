namespace Sdk.Connections.Contracts;

/// <summary>
/// A registry for managing different types of connections, their serializers, and tests.
/// </summary>
public interface IConnectionTypeRegistry
{
    /// <summary>
    /// Registers a new connection type with its associated components.
    /// </summary>
    void Register<TConnection, TConnectionSerializer>(string id, Func<IConnection> createConnection, TConnectionSerializer connectionSerializer, IConnectionTest? connectionTest)
        where TConnection : IConnection
        where TConnectionSerializer : IConnectionSerializer;

    /// <summary>
    /// Gets a read-only collection of all registered connection type identifiers.
    /// </summary>
    IReadOnlyCollection<string> GetConnectionTypes();

    /// <summary>
    /// Attempts to create a new instance of a connection for the specified type ID.
    /// </summary>
    bool TryCreateConnection(string id, [NotNullWhen(true)] out IConnection? connection);

    /// <summary>
    /// Attempts to retrieve the serializer for the specified connection type ID.
    /// </summary>
    bool TryGetConnectionSerializer(string id, [NotNullWhen(true)] out IConnectionSerializer? connectionSerializer);

    /// <summary>
    /// Attempts to retrieve the connection test for the specified connection type ID.
    /// </summary>
    bool TryGetConnectionTest(string id, [NotNullWhen(true)] out IConnectionTest? connectionTest);
}
