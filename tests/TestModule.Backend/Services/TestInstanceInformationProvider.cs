using System.Reflection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Sdk.Instance;
using Sdk.Modules;

namespace TestModule.Backend.Services;

/// <summary>
/// To test what happens if 2 Backend-Modules register implementations of the same interface 
/// </summary>
internal sealed class TestInstanceInformationProvider : IInstanceInformationProvider
{
    public IInstanceInformation Local { get; } = new TestInstanceInformation
    {
        Id = Guid.NewGuid(),
        Type = InstanceType.Standalone,
        Name = "TestInstance",
        InstalledModules = []
    };

    public event Func<Guid, HealthStatus, DateTime, Task>? HealthStatusChanged;

    public Task<IReadOnlyCollection<ModuleMetadata>> GetInstalledModules()
    {
        var sdkAssembly = Assembly.GetAssembly(typeof(IInstanceInformationProvider));
        var sdkName = sdkAssembly!.GetName() ?? throw new InvalidOperationException("Failed to retrieve sdk assembly");
        var sdkVersion = sdkName.Version?.ToString() ?? "0.0.0";

        var result = Local.InstalledModules
            .Select(k => new ModuleMetadata { Name = sdkName.Name!, Version = sdkVersion, MinSuiteSdkVersion = sdkVersion })
            .ToList()
            .AsReadOnly();

        return Task.FromResult<IReadOnlyCollection<ModuleMetadata>>(result);
    }

    public Task<List<IInstanceInformation>> GetInstancesInCluster(CancellationToken token)
        => Task.FromResult(new List<IInstanceInformation> { Local });

    public void TriggerHealthStatusChanged(Guid instanceId, HealthStatus status)
    => HealthStatusChanged?.Invoke(instanceId, status, DateTime.UtcNow);

    private sealed class TestInstanceInformation : IInstanceInformation
    {
        public Guid Id { get; set; }
        public InstanceType Type { get; set; }
        public string? Name { get; set; }
        public string FormattedName { get; set; } = "{ViciOne} Suite";
        public string? Description { get; set; }
        public string SerialNumber => Id.ToString("N");
        public IReadOnlyCollection<string> InstalledModules { get; set; } = [];
        public DateTime? FirstTimeRegistered { get; set; }
        public DateTime? LastRegistered { get; set; }
        public string Version { get; set; } = "undefined";
        public string? BranchName { get; init; }
        public string SdkVersion { get; set; } = "undefined";
        public bool InRecoveryMode { get; set; }
    }
}
