using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ViciOne.CodeAnalysis.MustCallBase.Attributes;

namespace Sdk.Testing.Backend;

public abstract class TestWithDbContextSqlite<TDbContext> : IDisposable where TDbContext : DbContext
{
    private readonly SqliteConnection _connection;
    // Track whether Dispose has been called. 
    private bool _disposed;

    protected TDbContext TestDbContext { get; }

    protected TestWithDbContextSqlite()
    {
        _connection = new SqliteConnection($"DataSource={TestDbContextFactory.DataSourceInMemory}");
        _connection.Open();

        TestDbContext = TestDbContextFactory.CreateSqliteContext<TDbContext>(_connection);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    [MustCallBase]
    protected virtual void Dispose(bool disposing)
    {
        // Check to see if Dispose has already been called. 
        if (_disposed)
            return;

        _connection.Close();

        if (disposing)
        {
            _connection.Dispose();
            TestDbContext.Dispose();
        }

        _disposed = true;
    }

    ~TestWithDbContextSqlite()
    {
        // Do not re-create Dispose clean-up code here. 
        // Calling Dispose(false) is optimal in terms of 
        // readability and maintainability.
        Dispose(false);
    }
}
