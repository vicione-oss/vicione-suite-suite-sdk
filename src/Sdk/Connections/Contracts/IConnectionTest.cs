namespace Sdk.Connections.Contracts;

/// <summary>
/// Defines a contract for testing a connection.
/// </summary>
public interface IConnectionTest
{
    /// <summary>
    /// Asynchronously tests the specified connection.
    /// </summary>
    Task<ConnectionTestResult> Test(IConnection connection, CancellationToken cancellationToken);
}
