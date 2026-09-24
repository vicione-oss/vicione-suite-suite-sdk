using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Sdk.Backend.Persistence;
using Xunit;

namespace Sdk.Backend.SourceGenerators.Tests;

public class ModuleDbContextGeneratorTests
{
    private static GeneratorDriverRunResult RunGenerator(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>()
            .ToList();

        // Sdk.Backend is not necessarily loaded yet, so it is referenced explicitly.
        references.Add(MetadataReference.CreateFromFile(typeof(ModuleDbContext).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(ModuleDbContextAttribute).Assembly.Location));

        var compilation = CSharpCompilation.Create(
            assemblyName: "TestAssembly",
            syntaxTrees: [syntaxTree],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new ModuleDbContextGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out _);

        return driver.GetRunResult();
    }

    [Fact]
    public void Should_generate_sqlite_and_postgres_classes()
    {
        // Arrange
        const string Source = """
            using Microsoft.EntityFrameworkCore;
            using Sdk.Backend.Persistence;

            namespace Test.DbContext;

            [ModuleDbContext]
            public partial class MyDbContext : ModuleDbContext
            {
                public override string DefaultSchemaName => "test";
            }
            """;

        // Act
        var result = RunGenerator(Source);

        // Assert
        result.Diagnostics.Should().BeEmpty();
        result.GeneratedTrees.Should().HaveCount(1);

        var generatedCode = result.GeneratedTrees[0].GetText(TestContext.Current.CancellationToken).ToString();
        generatedCode.Should().Contain("sealed class MyDbContextSqlite");
        generatedCode.Should().Contain("sealed class MyDbContextPostgres");
        generatedCode.Should().Contain(", ISqliteDbContext");
        generatedCode.Should().Contain(", IPostgresDbContext");
    }

    [Fact]
    public void Should_generate_design_time_factories()
    {
        // Arrange
        const string Source = """
            using Microsoft.EntityFrameworkCore;
            using Sdk.Backend.Persistence;

            namespace Test.DbContext;

            [ModuleDbContext]
            public partial class MyDbContext : ModuleDbContext
            {
                public override string DefaultSchemaName => "test";
            }
            """;

        // Act
        var result = RunGenerator(Source);

        // Assert
        var generatedCode = result.GeneratedTrees[0].GetText(TestContext.Current.CancellationToken).ToString();
        generatedCode.Should().Contain("sealed class MyDbContextSqliteFactory : IDesignTimeDbContextFactory<MyDbContextSqlite>");
        generatedCode.Should().Contain("sealed class MyDbContextPostgresFactory : IDesignTimeDbContextFactory<MyDbContextPostgres>");
        generatedCode.Should().Contain("optionsBuilder.UseSqlite()");
        generatedCode.Should().Contain("optionsBuilder.UseNpgsql()");
    }

    [Fact]
    public void Should_generate_constructor_when_none_declared()
    {
        // Arrange
        const string Source = """
            using Microsoft.EntityFrameworkCore;
            using Sdk.Backend.Persistence;

            namespace Test.DbContext;

            [ModuleDbContext]
            public partial class MyDbContext : ModuleDbContext
            {
                public override string DefaultSchemaName => "test";
            }
            """;

        // Act
        var result = RunGenerator(Source);

        // Assert
        var generatedCode = result.GeneratedTrees[0].GetText(TestContext.Current.CancellationToken).ToString();
        generatedCode.Should().Contain("protected MyDbContext(DbContextOptions options) : base(options) { }");
    }

    [Fact]
    public void Should_not_generate_constructor_when_user_declares_one()
    {
        // Arrange
        const string Source = """
            using Microsoft.EntityFrameworkCore;
            using Sdk.Backend.Persistence;

            namespace Test.DbContext;

            [ModuleDbContext]
            public partial class MyDbContext : ModuleDbContext
            {
                public override string DefaultSchemaName => "test";

                internal MyDbContext(DbContextOptions options) : base(options) { }
            }
            """;

        // Act
        var result = RunGenerator(Source);

        // Assert
        var generatedCode = result.GeneratedTrees[0].GetText(TestContext.Current.CancellationToken).ToString();
        generatedCode.Should().NotContain("protected MyDbContext(DbContextOptions options)");
    }

    [Fact]
    public void Should_generate_default_schema_name_when_set_in_attribute()
    {
        // Arrange
        const string Source = """
            using Microsoft.EntityFrameworkCore;
            using Sdk.Backend.Persistence;

            namespace Test.DbContext;

            [ModuleDbContext(DefaultSchemaName = "myschema")]
            public partial class MyDbContext : ModuleDbContext
            {
            }
            """;

        // Act
        var result = RunGenerator(Source);

        // Assert
        result.Diagnostics.Should().BeEmpty();

        var generatedCode = result.GeneratedTrees[0].GetText(TestContext.Current.CancellationToken).ToString();
        generatedCode.Should().Contain("""public override string DefaultSchemaName => "myschema";""");
    }

    [Fact]
    public void Should_not_generate_default_schema_name_when_not_set()
    {
        // Arrange
        const string Source = """
            using Microsoft.EntityFrameworkCore;
            using Sdk.Backend.Persistence;

            namespace Test.DbContext;

            [ModuleDbContext]
            public partial class MyDbContext : ModuleDbContext
            {
                public override string DefaultSchemaName => "manual";
            }
            """;

        // Act
        var result = RunGenerator(Source);

        // Assert
        var generatedCode = result.GeneratedTrees[0].GetText(TestContext.Current.CancellationToken).ToString();
        generatedCode.Should().NotContain("""DefaultSchemaName => "manual""");
    }

    [Fact]
    public void Should_use_internal_access_modifier_for_internal_class()
    {
        // Arrange
        const string Source = """
            using Microsoft.EntityFrameworkCore;
            using Sdk.Backend.Persistence;

            namespace Test.DbContext;

            [ModuleDbContext(DefaultSchemaName = "test")]
            internal partial class MyDbContext : ModuleDbContext
            {
            }
            """;

        // Act
        var result = RunGenerator(Source);

        // Assert
        var generatedCode = result.GeneratedTrees[0].GetText(TestContext.Current.CancellationToken).ToString();
        generatedCode.Should().Contain("internal sealed class MyDbContextSqlite");
        generatedCode.Should().Contain("internal sealed class MyDbContextPostgres");
        generatedCode.Should().Contain("internal partial class MyDbContext");
    }

    [Fact]
    public void Should_report_error_when_class_is_not_partial()
    {
        // Arrange
        const string Source = """
            using Microsoft.EntityFrameworkCore;
            using Sdk.Backend.Persistence;

            namespace Test.DbContext;

            [ModuleDbContext]
            public class MyDbContext : ModuleDbContext
            {
                public override string DefaultSchemaName => "test";
                internal MyDbContext(DbContextOptions options) : base(options) { }
            }
            """;

        // Act
        var result = RunGenerator(Source);

        // Assert
        result.Diagnostics.Should().ContainSingle()
            .Which.Id.Should().Be("SDKDBCTX001");
        result.GeneratedTrees.Should().BeEmpty();
    }

    [Fact]
    public void Should_report_error_when_class_does_not_derive_from_module_db_context()
    {
        // Arrange
        const string Source = """
            using Microsoft.EntityFrameworkCore;
            using Sdk.Backend.Persistence;

            namespace Test.DbContext;

            [ModuleDbContext]
            public partial class MyDbContext : DbContext
            {
                internal MyDbContext(DbContextOptions options) : base(options) { }
            }
            """;

        // Act
        var result = RunGenerator(Source);

        // Assert
        result.Diagnostics.Should().ContainSingle()
            .Which.Id.Should().Be("SDKDBCTX002");
        result.GeneratedTrees.Should().BeEmpty();
    }

    [Fact]
    public void Should_generate_dbset_properties_from_interface()
    {
        // Arrange
        const string Source = """
            using Microsoft.EntityFrameworkCore;
            using Sdk.Backend.Persistence;

            namespace Test.DbContext;

            public class MyEntity { public int Id { get; set; } }
            public class OtherEntity { public int Id { get; set; } }

            public interface IMyDbContext : IModuleDbContext
            {
                DbSet<MyEntity> MyEntities { get; }
                DbSet<OtherEntity> OtherEntities { get; }
            }

            [ModuleDbContext(DefaultSchemaName = "test")]
            public partial class MyDbContext : ModuleDbContext, IMyDbContext
            {
            }
            """;

        // Act
        var result = RunGenerator(Source);

        // Assert
        result.Diagnostics.Should().BeEmpty();

        var generatedCode = result.GeneratedTrees[0].GetText(TestContext.Current.CancellationToken).ToString();
        generatedCode.Should().Contain("DbSet<global::Test.DbContext.MyEntity> MyEntities => Set<global::Test.DbContext.MyEntity>();");
        generatedCode.Should().Contain("DbSet<global::Test.DbContext.OtherEntity> OtherEntities => Set<global::Test.DbContext.OtherEntity>();");
    }

    [Fact]
    public void Should_not_generate_dbset_property_already_declared_by_user()
    {
        // Arrange
        const string Source = """
            using Microsoft.EntityFrameworkCore;
            using Sdk.Backend.Persistence;

            namespace Test.DbContext;

            public class MyEntity { public int Id { get; set; } }
            public class OtherEntity { public int Id { get; set; } }

            public interface IMyDbContext : IModuleDbContext
            {
                DbSet<MyEntity> MyEntities { get; }
                DbSet<OtherEntity> OtherEntities { get; }
            }

            [ModuleDbContext(DefaultSchemaName = "test")]
            public partial class MyDbContext : ModuleDbContext, IMyDbContext
            {
                public DbSet<MyEntity> MyEntities => Set<MyEntity>();
            }
            """;

        // Act
        var result = RunGenerator(Source);

        // Assert
        var generatedCode = result.GeneratedTrees[0].GetText(TestContext.Current.CancellationToken).ToString();
        generatedCode.Should().NotContain("MyEntities");
        generatedCode.Should().Contain("OtherEntities");
    }

    [Fact]
    public void Should_generate_dbset_properties_from_every_level_of_a_layered_interface()
    {
        // Arrange
        const string Source = """
            using Microsoft.EntityFrameworkCore;
            using Sdk.Backend.Persistence;

            namespace Test.DbContext;

            public class MyEntity { public int Id { get; set; } }
            public class OtherEntity { public int Id { get; set; } }

            public interface IBaseDbContext : IModuleDbContext
            {
                DbSet<MyEntity> MyEntities { get; }
            }

            public interface IMyDbContext : IBaseDbContext
            {
                DbSet<OtherEntity> OtherEntities { get; }
            }

            [ModuleDbContext(DefaultSchemaName = "test")]
            public partial class MyDbContext : ModuleDbContext, IMyDbContext
            {
            }
            """;

        // Act
        var result = RunGenerator(Source);

        // Assert
        result.Diagnostics.Should().BeEmpty();

        var generatedCode = result.GeneratedTrees[0].GetText(TestContext.Current.CancellationToken).ToString();
        generatedCode.Should().Contain("DbSet<global::Test.DbContext.MyEntity> MyEntities => Set<global::Test.DbContext.MyEntity>();");
        generatedCode.Should().Contain("DbSet<global::Test.DbContext.OtherEntity> OtherEntities => Set<global::Test.DbContext.OtherEntity>();");
    }

    [Fact]
    public void Should_generate_dbset_properties_from_every_implemented_module_interface()
    {
        // Arrange
        const string Source = """
            using Microsoft.EntityFrameworkCore;
            using Sdk.Backend.Persistence;

            namespace Test.DbContext;

            public class MyEntity { public int Id { get; set; } }
            public class OtherEntity { public int Id { get; set; } }

            public interface IFirstDbContext : IModuleDbContext
            {
                DbSet<MyEntity> MyEntities { get; }
            }

            public interface ISecondDbContext : IModuleDbContext
            {
                DbSet<MyEntity> MyEntities { get; }
                DbSet<OtherEntity> OtherEntities { get; }
            }

            [ModuleDbContext(DefaultSchemaName = "test")]
            public partial class MyDbContext : ModuleDbContext, IFirstDbContext, ISecondDbContext
            {
            }
            """;

        // Act
        var result = RunGenerator(Source);

        // Assert
        result.Diagnostics.Should().BeEmpty();

        var generatedCode = result.GeneratedTrees[0].GetText(TestContext.Current.CancellationToken).ToString();
        generatedCode.Should().Contain("DbSet<global::Test.DbContext.OtherEntity> OtherEntities => Set<global::Test.DbContext.OtherEntity>();");
        generatedCode.Split("MyEntities =>").Should().HaveCount(2, "a property declared by two interfaces is generated once");
    }
}
