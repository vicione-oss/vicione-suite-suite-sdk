namespace Sdk.Connections.Contracts;

public sealed class DatabaseConnection : IConnection
{
    public string ConnectionString { get; set; } = string.Empty;

    public DatabaseConnectionType DatabaseType { get; set; }
}
