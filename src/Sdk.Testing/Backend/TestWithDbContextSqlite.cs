using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ViciOne.CodeAnalysis.MustCallBase.Attributes;

namespace Sdk.Testing.Backend;

/// <summary>
/// Base class giving each test instance its own in-memory SQLite database, kept alive by a connection held until dispose.
/// </summary>
public abstract class TestWithDbContextSqlite<TDbContext> : IDisposable, IAsyncDisposable
    where TDbContext : DbContext
{
    private readonly SqliteConnection _connection;
    private bool _disposed;

    /// <summary>
    /// Gets the context over the test's database, with the schema created from the model.
    /// </summary>
    protected TDbContext TestDbContext { get; }

    /// <summary>
    /// Opens the in-memory database and creates its schema.
    /// </summary>
    protected TestWithDbContextSqlite()
    {
        _connection = new SqliteConnection($"DataSource={TestDbContextFactory.DataSourceInMemory}");
        _connection.Open();

        TestDbContext = TestDbContextFactory.CreateSqliteContext<TDbContext>(_connection);
    }

    /// <inheritdoc/>
    [SuppressMessage("Design", "CA1063:Implement IDisposable Correctly",
        Justification = "The once-only guard sits in both entry points so that overrides of Dispose(bool) run at most once too.")]
    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref _disposed, true, false))
            return;

        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.CompareExchange(ref _disposed, true, false))
            return;

        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Asynchronous disposal hook for derived classes; runs at most once, and only when the test is disposed asynchronously.
    /// </summary>
    /// <remarks>
    /// <see cref="DisposeAsync"/> calls it first and then <see cref="Dispose(bool)"/> with <see langword="false"/>, so managed
    /// resources belong here and in the <c>disposing</c> branch of <see cref="Dispose(bool)"/>.
    /// </remarks>
    [MustCallBase]
    protected virtual async ValueTask DisposeAsyncCore()
    {
        await _connection.CloseAsync().ConfigureAwait(false);
        await _connection.DisposeAsync().ConfigureAwait(false);
        await TestDbContext.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Disposal hook for derived classes; runs at most once.
    /// </summary>
    /// <remarks>
    /// <paramref name="disposing"/> is <see langword="true"/> from <see cref="Dispose()"/>. From <see cref="DisposeAsync"/> it is
    /// <see langword="false"/>, because <see cref="DisposeAsyncCore"/> has already released the managed resources.
    /// </remarks>
    [MustCallBase]
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection.Dispose();
            TestDbContext.Dispose();
        }
    }
}
