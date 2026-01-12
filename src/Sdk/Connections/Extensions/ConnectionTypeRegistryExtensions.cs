using Sdk.Connections.Contracts;

namespace Sdk.Connections.Extensions;

public static class ConnectionTypeRegistryExtensions
{
    public static IConnectionTypeRegistry Register<TConnection, TConnectionSerializer>(this IConnectionTypeRegistry connectionTypeRegistry, string id, TConnectionSerializer connectionSerializer, IConnectionTest? connectionTest = default)
        where TConnection : IConnection, new()
        where TConnectionSerializer : IConnectionSerializer
    {
        connectionTypeRegistry.Register<TConnection, TConnectionSerializer>(id, () => new TConnection(), connectionSerializer, connectionTest);
        return connectionTypeRegistry;
    }

    public static IConnectionTypeRegistry Register<TConnection, TConnectionSerializer, TConnectionTest>(this IConnectionTypeRegistry connectionTypeRegistry, string id)
        where TConnection : IConnection, new()
        where TConnectionTest : IConnectionTest, new()
        where TConnectionSerializer : IConnectionSerializer, new()
        => connectionTypeRegistry.Register<TConnection, TConnectionSerializer>(id, new TConnectionSerializer(), new TConnectionTest());
}
