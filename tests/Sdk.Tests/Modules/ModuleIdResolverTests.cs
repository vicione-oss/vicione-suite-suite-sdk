using System.Reflection;
using System.Reflection.Emit;
using AwesomeAssertions;
using Sdk.Modules;
using TestModule.Backend;
using TestModule.Client;

namespace Sdk.Tests.Modules;

public sealed class ModuleIdResolverTests
{
    public sealed class ResolveId_FromString
    {
        [Theory]
        [InlineData("MyModule.Backend", "MyModule")]
        [InlineData("MyModule.Client", "MyModule")]
        [InlineData("MyModule.Internal", "MyModule")]
        [InlineData("MyModule.Public", "MyModule")]
        [InlineData("Acme.Backend.Tools.Backend", "Acme.Backend.Tools")]
        [InlineData("Acme.Client.Kit.Client", "Acme.Client.Kit")]
        [InlineData("Acme.Internal.Core.Internal", "Acme.Internal.Core")]
        [InlineData("Acme.Public.Api.Public", "Acme.Public.Api")]
        public void Should_remove_known_suffixes(string input, string expected)
        {
            // Arrange + Act
            var result = ModuleIdResolver.ResolveId(input);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void Should_throw_if_suffix_is_not_recognized()
        {
            // Arrange
            const string InvalidName = "MyModule.Unknown";

            // Act
            Action act = () => ModuleIdResolver.ResolveId(InvalidName);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }
    }

    public sealed class ResolveId_FromAssembly
    {
        [Fact]
        public void Should_resolve_id_from_assembly_name()
        {
            // Arrange
            const string AssemblyName = "SomeModule.Backend";
            var assembly = CreateAssemblyWithName(AssemblyName);

            // Act
            var result = ModuleIdResolver.ResolveId(assembly);

            // Assert
            result.Should().Be("SomeModule");
        }

        [Fact]
        public void Should_throw_if_assembly_name_is_null()
        {
            // Arrange
            var assembly = CreateAssemblyWithName(" ");

            // Act
            Action act = () => ModuleIdResolver.ResolveId(assembly);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        private static AssemblyBuilder CreateAssemblyWithName(string? name)
        {
            var assemblyName = new AssemblyName { Name = name };
            return AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        }
    }

    public sealed class ResolveId_FromType
    {
        [Fact]
        public void Should_resolve_id_for_backend_type()
        {
            // Arrange
            var type = typeof(TestBackendModule);

            // Act
            var result = ModuleIdResolver.ResolveId(type);

            // Assert
            result.Should().Be("ViciOne.Suite.TestModule");
        }

        [Fact]
        public void Should_resolve_id_for_client_type()
        {
            // Arrange
            var type = typeof(TestClientModule);

            // Act
            var result = ModuleIdResolver.ResolveId(type);

            // Assert
            result.Should().Be("ViciOne.Suite.TestModule");
        }

        [Theory]
        [InlineData("Acme.Backend.Tools.Backend", "Acme.Backend.Tools")]
        [InlineData("Acme.Client.Kit.Client", "Acme.Client.Kit")]
        public void Should_remove_only_the_trailing_suffix(string assemblyName, string expected)
        {
            // Arrange
            var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
            var type = assembly.DefineDynamicModule(assemblyName).DefineType("Module").CreateType();

            // Act
            var result = ModuleIdResolver.ResolveId(type);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void Should_throw_for_invalid_type()
        {
            // Arrange
            var type = typeof(string); // assembly name has no suffix

            // Act
            Action act = () => ModuleIdResolver.ResolveId(type);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }
    }

    public sealed class GetModuleName
    {
        [Theory]
        [InlineData("Suite.Core.MyModule", "MyModule")]
        [InlineData("MyCompany.Product.ModuleX", "ModuleX")]
        public void Should_return_last_segment_of_module_id(string input, string expected)
        {
            // Arrange + Act
            var result = ModuleIdResolver.GetModuleName(input);

            // Assert
            result.Should().Be(expected);
        }
    }
}
