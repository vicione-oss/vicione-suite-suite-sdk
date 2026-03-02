namespace Sdk.Connections.Contracts;

/// <summary>
/// Represents a contract for a SQLite-specific connection configuration data.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class SQLiteConnection : IConnection
{
    /// <summary>
    /// Gets or sets the connection string of the SQLite database connection.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
