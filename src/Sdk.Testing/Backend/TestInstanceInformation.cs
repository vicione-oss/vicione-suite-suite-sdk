using Sdk.Instance;

namespace Sdk.Testing.Backend;

public class TestInstanceInformation : IInstanceInformation
{
    public Guid Id { get; init; }
    public InstanceType Type { get; init; }
    public string? Name { get; init; }
    public string FormattedName { get; } = "{ViciOne} Suite";
    public string? Description { get; init; }
    public string SerialNumber => Id.ToString("N");
    public IReadOnlyCollection<string> InstalledModules { get; init; } = [];
    public DateTime? FirstTimeRegistered { get; } = DateTime.UtcNow;
    public DateTime? LastRegistered { get; } = DateTime.UtcNow;
    public string Version { get; init; } = "undefined";
    public string? BranchName { get; init; }
    public string SdkVersion { get; init; } = "undefined";
    public bool InRecoveryMode { get; set; }
}
