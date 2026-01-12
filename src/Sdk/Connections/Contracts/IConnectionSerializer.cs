namespace Sdk.Connections.Contracts;

/// <summary>
/// Defines a contract for serializing and deserializing connection objects.
/// </summary>
public interface IConnectionSerializer
{
    /// <summary>
    /// Serializes the specified connection object into a string representation.
    /// </summary>
    string Serialize(IConnection connection);

    /// <summary>
    /// Deserializes a string into a connection object.
    /// </summary>
    IConnection? Deserialize(string serializedConnection);
}
