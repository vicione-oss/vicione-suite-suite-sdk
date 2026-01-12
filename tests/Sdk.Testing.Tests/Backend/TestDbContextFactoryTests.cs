using AwesomeAssertions;
using Microsoft.Data.Sqlite;
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
        dbContext.Db.Should().NotBeNull();
    }

    [Fact]
    public void Should_create_instance_from_connection()
    {
        //
        using var connection = new SqliteConnection($@"Data Source={TestDbContextFactory.DataSourceInMemory};");
        connection.Open();

        // Arrange + Act
        using var dbContext = TestDbContextFactory.CreateSqliteContext<TheDbContextSqlite>(connection);

        // Assert
        dbContext.Db.Should().NotBeNull();
    }
}
