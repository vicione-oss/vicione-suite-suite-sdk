namespace Sdk.Connections.Contracts;

/// <summary>
/// The configuration of a PostgreSQL connection.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class PostgresConnection : IConnection
{
    /// <summary>
    /// Gets or sets the connection string of the Postgres database connection.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
