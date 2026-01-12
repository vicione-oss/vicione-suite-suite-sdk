namespace Sdk.Connections.Contracts;

public interface IConnectionTest
{
    Task<ConnectionTestResult> Test(IConnection connection, CancellationToken cancellationToken);
}
