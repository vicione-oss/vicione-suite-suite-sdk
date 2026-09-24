using Sdk.Modules;

namespace Sdk.Client.Modules;

/// <summary>
/// Marks a client module; derive from <see cref="ClientModule"/> rather than implementing it directly.
/// </summary>
public interface IClientModule : IModule;
