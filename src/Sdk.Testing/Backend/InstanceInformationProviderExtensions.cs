using Sdk.Instance;

namespace Sdk.Testing.Backend;

/// <summary>
/// Provides extension methods for <see cref="IInstanceInformationProvider"/> to simplify mocking for testing.
/// </summary>
public static class InstanceInformationProviderExtensions
{
    /// <summary>
    /// The ID every master instance set up by these extensions gets, so tests can target it.
    /// </summary>
    public static readonly Guid TestMasterInstanceGuid = Guid.Parse("6151CF7C-0FBB-44DF-9CAC-D719C62315C9");

    extension(IInstanceInformationProvider instanceProvider)
    {
        /// <summary>
        /// Makes the substitute report a single local instance of <paramref name="type"/> with a random ID, or
        /// <see cref="TestMasterInstanceGuid"/> for a master.
        /// </summary>
        public void SetupGetInstanceInformation(InstanceType type, IEnumerable<string>? installedModules = default)
            => instanceProvider.SetupGetInstanceInformation(Guid.NewGuid(), type, installedModules);

        /// <summary>
        /// Makes the substitute report a single local instance, which is also the whole cluster.
        /// </summary>
        /// <param name="instanceId">
        /// The instance ID; ignored for <see cref="InstanceType.Master"/>, which always gets <see cref="TestMasterInstanceGuid"/>.
        /// </param>
        /// <param name="type">The instance type.</param>
        /// <param name="installedModules">The module IDs reported as installed; <see langword="null"/> means none.</param>
        public void SetupGetInstanceInformation(Guid instanceId, InstanceType type, IEnumerable<string>? installedModules = default)
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
}
