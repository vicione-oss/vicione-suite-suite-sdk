namespace Sdk.Connections.Contracts;

/// <summary>
/// Represents the association between a connection and a tag.
/// </summary>
public sealed class ConnectionTag
{
    /// <summary>
    /// Gets or sets the unique identifier of the connection.
    /// </summary>
    public Guid ConnectionId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the tag.
    /// </summary>
    public Guid TagId { get; set; }
}
