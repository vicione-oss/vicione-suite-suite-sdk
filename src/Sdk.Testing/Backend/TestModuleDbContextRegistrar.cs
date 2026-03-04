using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Sdk.Backend.Persistence;
namespace Sdk.Testing.Backend;
/// <summary>
/// A test implementation of <see cref="IModuleDbContextRegistrar"/> that registers in-memory SQLite contexts.
/// Used by test helpers so that modules calling <c>AddModuleDbContext</c> work without a real host application.
/// </summary>
public sealed class TestModuleDbContextRegistrar : IModuleDbContextRegistrar, IAsyncDisposable
{
    private readonly Dictionary<string, SqliteConnection> _keeperConnections = [];

    /// <inheritdoc/>
    public void Register<TDbContextInterface, TSqliteImplementation, TPostgresImplementation>(
        IServiceCollection services,
        string moduleId,
        Type moduleType,
        string sqliteDbName,
        bool enableSynchronization)
        where TDbContextInterface : IModuleDbContext
        where TSqliteImplementation : DbContext, ISqliteDbContext, TDbContextInterface
        where TPostgresImplementation : DbContext, IPostgresDbContext, TDbContextInterface
    {
        if (!_keeperConnections.ContainsKey(sqliteDbName))
        {
            var connection = new SqliteConnection($"Data Source={sqliteDbName};Mode=Memory;Cache=Shared");
            connection.Open();
            _keeperConnections[sqliteDbName] = connection;
        }

        services.AddSingleton(new ModuleContextTypeInformation(moduleId,
            typeof(TDbContextInterface),
            typeof(TDbContextInterface).FullName!));
        services.AddScoped(typeof(TDbContextInterface), _ =>
        {
            var connection = new SqliteConnection($"Data Source={sqliteDbName};Mode=Memory;Cache=Shared");
            try
            {
                connection.Open();
                return TestDbContextFactory.CreateSqliteContext<TSqliteImplementation>(connection, init: false);
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        });
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        foreach (var connection in _keeperConnections.Values)
            await connection.DisposeAsync();
    }
}
