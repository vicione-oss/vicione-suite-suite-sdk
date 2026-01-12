using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Sdk.Backend.Modules;

namespace Sdk.Backend.Persistence;

public sealed class DbContextResolver<TSqliteDbContext, TPostgresDbContext, TDbContextBaseInterface>(DbContextResolverOptions<TDbContextBaseInterface> options)
    where TSqliteDbContext : DbContext, ISqliteDbContext, TDbContextBaseInterface
    where TPostgresDbContext : DbContext, IPostgresDbContext, TDbContextBaseInterface
    where TDbContextBaseInterface : IModuleDbContext
{
    private readonly DbContextResolverOptions<TDbContextBaseInterface> _options = options;

    public TDbContextBaseInterface Resolve(IServiceProvider services)
    {
        if (services.GetService(typeof(IMasterDbConnectionStringProvider)) is not IMasterDbConnectionStringProvider connectionStringProvider)
        {
            return ActivatorUtilities.CreateInstance<TSqliteDbContext>(services,
                new DbContextOptionsBuilder<TSqliteDbContext>()
                    .UseSqlite(GetSqliteConnectionString(services))
                    .Options);
        }

        var optionsBuilder = new DbContextOptionsBuilder<TPostgresDbContext>()
            .UseNpgsql(connectionStringProvider.ConnectionString);

        if (_options.EnableSynchronization)
            optionsBuilder.AddInterceptors(services.GetRequiredService<ISaveChangesInterceptor>());

        return ActivatorUtilities.CreateInstance<TPostgresDbContext>(services, optionsBuilder.Options);
    }

    private string GetSqliteConnectionString(IServiceProvider services)
    {
        var serviceType = typeof(IWorkspaceProvider<>).MakeGenericType(_options.ModuleType);
        var wsService = services.GetRequiredService(serviceType);
        var workspace = (string)serviceType
            .GetProperty(nameof(IWorkspaceProvider<BackendModule>.Home))!
            .GetValue(wsService)!; // never null, b/c we only use references and require the service

        // if no dbname specified we have module id as dbname + ".db"
        var sqliteDbName = _options.DbName;
        if (!sqliteDbName.EndsWith(".db", StringComparison.OrdinalIgnoreCase))
            sqliteDbName += ".db";

        return new SqliteConnectionStringBuilder
        {
            DataSource = Path.Combine(workspace, sqliteDbName)
        }.ConnectionString;
    }
}
