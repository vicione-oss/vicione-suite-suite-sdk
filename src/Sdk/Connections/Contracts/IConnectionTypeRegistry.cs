using System.Diagnostics.CodeAnalysis;

namespace Sdk.Connections.Contracts;

public interface IConnectionTypeRegistry
{
    void Register<TConnection, TConnectionSerializer>(string id, Func<IConnection> createConnection, TConnectionSerializer connectionSerializer, IConnectionTest? connectionTest)
        where TConnection : IConnection
        where TConnectionSerializer : IConnectionSerializer;

    IReadOnlyCollection<string> GetConnectionTypes();

    bool TryCreateConnection(string id, [NotNullWhen(true)] out IConnection? connection);
    bool TryGetConnectionSerializer(string id, [NotNullWhen(true)] out IConnectionSerializer? connectionSerializer);
    bool TryGetConnectionTest(string id, [NotNullWhen(true)] out IConnectionTest? connectionTest);
}
