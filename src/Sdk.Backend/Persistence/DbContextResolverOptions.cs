using System.Diagnostics.CodeAnalysis;

namespace Sdk.Backend.Persistence;

/// <summary>
/// Provides configuration options for resolving a database context (<typeparamref name="TContextType"/>)
/// associated with a specific module, including its type, database name, and synchronization behavior.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class DbContextResolverOptions<TContextType>(Type moduleType, string dbName, bool enableSynchronization = true)
    where TContextType : IModuleDbContext
{
    /// <summary>
    /// <see cref="Type"/> of the module that owns the database context.
    /// </summary>
    public Type ModuleType { get; } = moduleType;

    /// <summary>
    /// Name of the database associated with the module.
    /// </summary>
    public string DbName { get; } = dbName;

    /// <summary>
    /// Value indicating whether schema synchronization is enabled for this database context.
    /// </summary>
    /// <remarks>
    /// When <see langword="true"/>, automatic synchronization on master-slave systems is enabled.
    /// When <see langword="false"/>, synchronization is skipped (e.g. local instance db).
    /// </remarks>
    public bool EnableSynchronization { get; } = enableSynchronization;
}
