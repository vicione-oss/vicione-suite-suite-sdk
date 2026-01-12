using NSubstitute;
using Sdk.Instance;

namespace Sdk.Testing.Backend;

public static class InstanceInformationProviderExtensions
{
    public static readonly Guid TestMasterInstanceGuid = Guid.Parse("6151CF7C-0FBB-44DF-9CAC-D719C62315C9");

    public static void SetupGetInstanceInformation(this IInstanceInformationProvider instanceProvider, InstanceType type, IEnumerable<string>? installedModules = default)
        => instanceProvider.SetupGetInstanceInformation(Guid.NewGuid(), type, installedModules);

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
