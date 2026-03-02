namespace Sdk.Connections.Contracts;

/// <summary>
/// Represents a type of connection in a strongly-typed, extensible manner.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class ConnectionType(string name) : IEquatable<ConnectionType>
{
    /// <summary>
    /// Gets the name of the connection type.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets a connection type representing SQLite connections.
    /// </summary>
    public static ConnectionType SQLite => new("SQLite");

    /// <summary>
    /// Gets a connection type representing Postgres connections.
    /// </summary>
    public static ConnectionType Postgres => new("Postgres");

    /// <summary>
    /// Gets a connection type representing HTTP/HTTPS connections.
    /// </summary>
    public static ConnectionType Http => new("http");

    /// <summary>
    /// Gets a connection type representing MQTT protocol connections.
    /// </summary>
    public static ConnectionType Mqtt => new("mqtt");

    /// <summary>
    /// Implicitly converts a <see cref="ConnectionType"/> to its string representation.
    /// </summary>
    public static implicit operator string(ConnectionType connectionType)
        => connectionType.ToString();

    /// <summary>
    /// Returns the string representation of the connection type.
    /// </summary>
    public override string ToString()
        => Name;

    /// <summary>
    /// Determines whether this instance is equal to another <see cref="ConnectionType"/> instance.
    /// </summary>
    public bool Equals(ConnectionType? other)
        => Name.Equals(other?.Name, StringComparison.Ordinal);

    /// <summary>
    /// Determines whether this instance is equal to a specified object.
    /// </summary>
    public override bool Equals(object? obj)
        => Equals(obj as ConnectionType);

    /// <summary>
    /// Returns the hash code for this connection type.
    /// </summary>
    public override int GetHashCode()
        => Name.GetHashCode(StringComparison.Ordinal);

    /// <summary>
    /// Determines whether two <see cref="ConnectionType"/> instances are equal.
    /// </summary>
    public static bool operator ==(ConnectionType? connectionType1, ConnectionType? connectionType2)
        => connectionType1?.Equals(connectionType2) ?? false;

    /// <summary>
    /// Determines whether two <see cref="ConnectionType"/> instances are not equal.
    /// </summary>
    public static bool operator !=(ConnectionType? connectionType1, ConnectionType? connectionType2)
        => !(connectionType1 == connectionType2);
}
