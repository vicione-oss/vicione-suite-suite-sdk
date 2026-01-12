using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ViciOne.CodeAnalysis.MustCallBase.Attributes;

namespace Sdk.Testing.Backend;

/// <summary>
/// An abstract base class for tests that require an in-memory SQLite database context.
/// </summary>
public abstract class TestWithDbContextSqlite<TDbContext> : IDisposable, IAsyncDisposable
    where TDbContext : DbContext
{
    private readonly SqliteConnection _connection;
    private bool _disposed;

    /// <summary>
    /// Gets the DbContext instance for use in tests.
    /// </summary>
    protected TDbContext TestDbContext { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestWithDbContextSqlite{TDbContext}"/> class.
    /// </summary>
    protected TestWithDbContextSqlite()
    {
        _connection = new SqliteConnection($"DataSource={TestDbContextFactory.DataSourceInMemory}");
        _connection.Open();

        TestDbContext = TestDbContextFactory.CreateSqliteContext<TDbContext>(_connection);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        // Attempt to mark as disposed; if already disposed, no-op.
        if (Interlocked.CompareExchange(ref _disposed, true, false))
            return;

        // Ensure the connection is closed before disposing.
        await _connection.CloseAsync().ConfigureAwait(false);

        await _connection.DisposeAsync().ConfigureAwait(false);
        await TestDbContext.DisposeAsync().ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Provides a hook for derived classes to perform their own disposal logic.
    /// </summary>
    /// <remarks>
    /// This method is called from within <see cref="Dispose()"/>.
    /// </remarks>
    [MustCallBase]
    protected virtual void Dispose(bool disposing)
    {
        // Atomically set disposed flag; if already set, return.
        if (Interlocked.CompareExchange(ref _disposed, true, false))
            return;

        _connection.Close();

        if (disposing)
        {
            _connection.Dispose();
            TestDbContext.Dispose();
        }
    }
}
