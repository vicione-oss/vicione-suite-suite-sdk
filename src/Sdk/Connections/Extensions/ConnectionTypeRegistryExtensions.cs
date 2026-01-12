using Sdk.Connections.Contracts;

namespace Sdk.Connections.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IConnectionTypeRegistry"/> to simplify connection type registration.
/// </summary>
public static class ConnectionTypeRegistryExtensions
{
    /// <summary>
    /// Registers a connection type with an explicit serializer instance and optional connection test.
    /// </summary>
    public static IConnectionTypeRegistry Register<TConnection, TConnectionSerializer>(this IConnectionTypeRegistry connectionTypeRegistry, string id, TConnectionSerializer connectionSerializer, IConnectionTest? connectionTest = default)
        where TConnection : IConnection, new()
        where TConnectionSerializer : IConnectionSerializer
    {
        connectionTypeRegistry.Register<TConnection, TConnectionSerializer>(id, () => new TConnection(), connectionSerializer, connectionTest);
        return connectionTypeRegistry;
    }

    /// <summary>
    /// Registers a connection type using default constructors for connection, serializer, and test.
    /// </summary>
    public static IConnectionTypeRegistry Register<TConnection, TConnectionSerializer, TConnectionTest>(this IConnectionTypeRegistry connectionTypeRegistry, string id)
        where TConnection : IConnection, new()
        where TConnectionTest : IConnectionTest, new()
        where TConnectionSerializer : IConnectionSerializer, new()
        => connectionTypeRegistry.Register<TConnection, TConnectionSerializer>(id, new TConnectionSerializer(), new TConnectionTest());
}
