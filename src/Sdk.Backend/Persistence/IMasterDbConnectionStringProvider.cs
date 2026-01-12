namespace Sdk.Backend.Persistence;

/// <summary>
/// Defines a provider for the master database connection string.
/// </summary>
public interface IMasterDbConnectionStringProvider
{
    /// <summary>
    /// Gets the connection string for the master database.
    /// </summary>
    string ConnectionString { get; }
}
