namespace Sdk.Connections.Contracts;

/// <summary>
/// Represents a contract for a Postgres-specific connection configuration data.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class PostgresConnection : IConnection
{
    /// <summary>
    /// Gets or sets the connection string of the Postgres database connection.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
