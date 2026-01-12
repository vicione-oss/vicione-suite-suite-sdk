namespace Sdk.Backend.Persistence;

/// <summary>
/// Encapsulates type information about a module's implementation of <see cref="IModuleDbContext"/>.
/// </summary>
public sealed record ModuleContextTypeInformation(string ModuleId, Type ContextType, string FullName);
