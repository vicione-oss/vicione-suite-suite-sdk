namespace Sdk.Connections.Contracts;

/// <summary>
/// Represents a connection with associated metadata, tags, and configuration details.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class Connection
{
    /// <summary>
    /// Gets or sets the raw JSON representation of the connection, if available.
    /// </summary>
    public string? Json { get; set; }

    /// <summary>
    /// Gets or sets a human-readable description of the connection.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or initializes the unique identifier for this connection.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the name of the connection.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the type of the connection.
    /// Defaults to <see cref="ConnectionType.Http"/>.
    /// </summary>
    public ConnectionType Type { get; set; } = ConnectionType.Http;

    /// <summary>
    /// Gets or sets the set of tags associated with this connection.
    /// </summary>
    public HashSet<Tag> Tags { get; set; } = [];

    /// <summary>
    /// Gets or sets additional metadata for the connection as key/value pairs.
    /// </summary>
    public Dictionary<string, string?> Metadata { get; set; } = [];

    /// <summary>
    /// Gets or sets whether this connection is managed.
    /// </summary>
    /// <remarks>
    /// Managed connections cannot be deleted by the user.
    /// </remarks>
    public bool Managed { get; set; }
}
