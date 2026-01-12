namespace Sdk.Connections.Contracts;

public sealed class Connection
{
    public string? Json { get; set; }
    public string? Description { get; set; }
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public ConnectionType Type { get; set; } = ConnectionType.AzureIotHub;
    public HashSet<Tag> Tags { get; set; } = [];
    public Dictionary<string, string?> Metadata { get; set; } = [];
}
