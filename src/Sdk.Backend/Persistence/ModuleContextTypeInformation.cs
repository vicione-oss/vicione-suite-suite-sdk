using System.Diagnostics.CodeAnalysis;

namespace Sdk.Backend.Persistence;

/// <summary>
/// Encapsulates type information about a module's implementation of <see cref="IModuleDbContext"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record ModuleContextTypeInformation(string ModuleId, Type ContextType, string FullName);
