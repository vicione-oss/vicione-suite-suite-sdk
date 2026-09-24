using System.Data;
using AwesomeAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Sdk.Testing.Backend;
using Xunit;

namespace Sdk.Testing.Tests.Backend;

public class TestDbContextFactoryTests
{
    [Fact]
    public void Should_create_an_in_memory_instance()
    {
        // Arrange + Act
        using var dbContext = TestDbContextFactory.CreateSqliteContext<TheDbContextSqlite>();

        // Assert
        dbContext.ChangeTracker.Should().NotBeNull();
    }

    [Fact]
    public void Should_create_instance_from_connection()
    {
        // Arrange
        using var connection = new SqliteConnection($"Data Source={TestDbContextFactory.DataSourceInMemory};");
        connection.Open();

        // Act
        using var dbContext = TestDbContextFactory.CreateSqliteContext<TheDbContextSqlite>(connection);

        // Assert
        dbContext.ChangeTracker.Should().NotBeNull();
    }

    [Fact]
    public void Should_close_its_own_connection_when_the_context_is_disposed()
    {
        // Arrange
        var dbContext = TestDbContextFactory.CreateSqliteContext<TheDbContextSqlite>(TestDbContextFactory.DataSourceInMemory);
        var connection = dbContext.Database.GetDbConnection();

        // Act
        dbContext.Dispose();

        // Assert
        connection.State.Should().Be(ConnectionState.Closed);
    }

    [Fact]
    public void Should_leave_a_supplied_connection_open_when_the_context_is_disposed()
    {
        // Arrange
        using var connection = new SqliteConnection($"Data Source={TestDbContextFactory.DataSourceInMemory};");
        connection.Open();
        var dbContext = TestDbContextFactory.CreateSqliteContext<TheDbContextSqlite>(connection);

        // Act
        dbContext.Dispose();

        // Assert
        connection.State.Should().Be(ConnectionState.Open);
    }
}
