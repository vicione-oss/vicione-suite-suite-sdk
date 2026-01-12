namespace Sdk.Connections.Contracts;

public sealed class ConnectionType(string name) : IEquatable<ConnectionType>
{
    public string Name { get; private set; } = name;

    public static ConnectionType AzureIotHub => new("azure.iothub");
    public static ConnectionType Database => new("database");
    public static ConnectionType Http => new("http");
    public static ConnectionType Mqtt => new("mqtt");

    public static implicit operator string(ConnectionType connectionType)
        => connectionType.ToString();

    public override string ToString()
        => Name;

    public bool Equals(ConnectionType? other)
        => Name.Equals(other?.Name, StringComparison.Ordinal);

    public override bool Equals(object? obj)
        => Equals(obj as ConnectionType);

    public override int GetHashCode()
        => Name.GetHashCode(StringComparison.Ordinal);

    public static bool operator ==(ConnectionType? connectionType1, ConnectionType? connectionType2)
        => connectionType1?.Equals(connectionType2) ?? false;

    public static bool operator !=(ConnectionType? connectionType1, ConnectionType? connectionType2)
        => !(connectionType1 == connectionType2);
}
