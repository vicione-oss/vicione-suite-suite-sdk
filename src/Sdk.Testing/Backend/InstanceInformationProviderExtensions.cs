using NSubstitute;
using Sdk.Instance;

namespace Sdk.Testing.Backend;

/// <summary>
/// Provides extension methods for <see cref="IInstanceInformationProvider"/> to simplify mocking for testing.
/// </summary>
public static class InstanceInformationProviderExtensions
{
    /// <summary>
    /// A constant GUID used to represent the master instance in tests.
    /// </summary>
    public static readonly Guid TestMasterInstanceGuid = Guid.Parse("6151CF7C-0FBB-44DF-9CAC-D719C62315C9");

    /// <summary>
    /// Sets up a mocked <see cref="IInstanceInformationProvider"/> to return test instance information with a new random GUID.
    /// </summary>
    public static void SetupGetInstanceInformation(this IInstanceInformationProvider instanceProvider, InstanceType type, IEnumerable<string>? installedModules = default)
        => instanceProvider.SetupGetInstanceInformation(Guid.NewGuid(), type, installedModules);

    /// <summary>
    /// Sets up a mocked <see cref="IInstanceInformationProvider"/> to return test instance information with a specific GUID.
    /// </summary>
    public static void SetupGetInstanceInformation(this IInstanceInformationProvider instanceProvider, Guid instanceId, InstanceType type, IEnumerable<string>? installedModules = default)
    {
        var info = new TestInstanceInformation
        {
            Id = type == InstanceType.Master ? TestMasterInstanceGuid : instanceId,
            Type = type,
            Name = "Test",
            InstalledModules = installedModules?.ToList() ?? []
        };

        instanceProvider.Local
            .Returns(info);

        instanceProvider
            .GetInstancesInCluster(Arg.Any<CancellationToken>())
            .Returns([info]);
    }
}
