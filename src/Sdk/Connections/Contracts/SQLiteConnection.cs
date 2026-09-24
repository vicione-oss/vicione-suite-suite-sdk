namespace Sdk.Connections.Contracts;

/// <summary>
/// The configuration of a SQLite connection.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class SQLiteConnection : IConnection
{
    /// <summary>
    /// Gets or sets the connection string of the SQLite database connection.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
