namespace Sdk.Connections.Contracts;

public interface IConnectionSerializer
{
    string Serialize(IConnection connection);
    IConnection? Deserialize(string serializedConnection);
}
