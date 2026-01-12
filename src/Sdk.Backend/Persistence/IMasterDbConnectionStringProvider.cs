namespace Sdk.Backend.Persistence;

public interface IMasterDbConnectionStringProvider
{
    /// <summary>
    /// Master Database connection string.
    /// </summary>
    string ConnectionString { get; }
}
