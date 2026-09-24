using System.Reflection;
using AwesomeAssertions;
using Sdk.Client.Modules;
using Sdk.Extensions;
using Sdk.Messaging;
using Sdk.Modules;
using TestModule.Backend;
using TestModule.Client;

namespace Sdk.Tests.Extensions;

public class ReflectionExtensionTests
{
    public sealed class GetImplementingTypes : ReflectionExtensionTests
    {
        [Fact]
        public void Should_return_public_implementing_interface_types_from_assembly()
        {
            // Arrange
            var assembly = typeof(TestBackendModule).Assembly;

            // Act
            var module = assembly.GetImplementingTypes<IModule>();

            // Assert
            module.Should().ContainSingle(k => k == typeof(TestBackendModule));
        }

        [Fact]
        public void Should_return_public_implementing_interface_types_from_assemblies()
        {
            // Arrange
            var assemblies = new List<Assembly>() {
                typeof(TestBackendModule).Assembly,
                typeof(TestClientModule).Assembly
            };

            // Act
            var modules = assemblies.GetImplementingTypes<IModule>().ToList();

            // Assert
            modules.Should().ContainSingle(k => k == typeof(TestBackendModule));
            modules.Should().ContainSingle(k => k == typeof(TestClientModule));
        }
    }

    public sealed class GetInstances : ReflectionExtensionTests
    {
        [Fact]
        public void Should_return_instances_for_implementing_interface_types_from_assemblies()
        {
            // Arrange
            var assemblies = new List<Assembly>() {
                typeof(TestBackendModule).Assembly,
                typeof(TestClientModule).Assembly
            };

            // Act
            var modules = assemblies.GetInstances<IModule>().ToList();

            // Assert
            modules.Should().ContainSingle(k => k is TestBackendModule);
            modules.Should().ContainSingle(k => k is TestClientModule);
        }

        [Fact]
        public void Should_return_only_requested_instances_for_implementing_interface_types_from_assemblies()
        {
            // Arrange
            var assemblies = new List<Assembly>() {
                typeof(TestBackendModule).Assembly,
                typeof(TestClientModule).Assembly,
                typeof(TestOtherEditorClientModule).Assembly,
            };

            // Act
            var modules = assemblies.GetInstances<ClientModule>();

            // Assert
            modules.Should().AllSatisfy(k => k.Should().BeAssignableTo<ClientModule>());
        }
    }

    public sealed class GetFirstInstance : ReflectionExtensionTests
    {
        [Fact]
        public void Should_return_first_public_implementing_interface_types_from_assemblies()
        {
            // Arrange
            var assembly = typeof(TestBackendModule).Assembly;

            // Act
            var module = assembly.GetFirstInstance<IModule>();

            // Assert
            module.Should().BeOfType<TestBackendModule>();
        }
    }

    public sealed class GetGenericTypeName : ReflectionExtensionTests
    {
        [Fact]
        public void TypeIsNull()
        {
            Type? type = null;
            var typename = type.GetGenericTypeName();
            typename.Should().Be("null");
        }

        [Fact]
        public void TypeIsNotGeneric()
        {
            var type = typeof(int);
            var typename = type.GetGenericTypeName();
            typename.Should().Be(type.Name);
        }

        [Fact]
        public void TypeIsGeneric()
        {
            var type = typeof(List<int>);
            var typename = type.GetGenericTypeName();
            typename.Should().Be("List[Int32]");
        }
    }

    public sealed class GetAttributeValue : ReflectionExtensionTests
    {
        [Fact]
        public void Should_return_first_attribute_resolved_value()
        {
            // Arrange
            var assembly = typeof(ReflectionExtensionTests).Assembly;
            var company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()!.Company;

            // Act
            var value = assembly.GetAttributeValue<AssemblyCompanyAttribute>(a => a.Company, "default");

            // Assert
            value.Should().Be(company).And.NotBe("default");
        }

        [Fact]
        public void Should_return_default_when_attribute_is_missing()
        {
            // Arrange
            var assembly = typeof(ReflectionExtensionTests).Assembly;

            // Act
            var value = assembly.GetAttributeValue<ForwardToUIAttribute>(_ => "resolved", "default");

            // Assert
            value.Should().Be("default");
        }
    }

    public sealed class IsNullable : ReflectionExtensionTests
    {
        [Theory]
        [InlineData(typeof(FooPropertyInt))]        // 1. Property - public int _Int1 { get; set; }
        [InlineData(typeof(FooPropertyString))]     // 1. Property - public string Str1 { get; set; } = "";
        [InlineData(typeof(FooPropertyList))]       // 1. Property - public List<Foo> Lst1 { get; set; } = new();
        [InlineData(typeof(FooPropertyClass))]      // 1. Property - public FooNode FooNode1 { get; set; } = new();
        public void Should_return_false_for_not_nullable_property(Type type)
        {
            // Arrange
            var property = type.GetProperties()[0];

            // Act
            var isNullable = property.IsNullable();

            // Assert
            isNullable.Should().BeFalse();
        }

        [Theory]
        [InlineData(typeof(FooPropertyInt))]        // 2. Property - public int? Int2 { get; set; } = null;
        [InlineData(typeof(FooPropertyString))]     // 2. Property - public string? Str2 { get; set; } = null;
        [InlineData(typeof(FooPropertyList))]       // 2. Property - public List<Foo>? Lst2 { get; set; } = null;
        public void Should_return_true_for_nullable_property(Type type)
        {
            // Arrange
            var property = type.GetProperties()[1];

            // Act
            var isNullable = property.IsNullable();

            // Assert
            isNullable.Should().BeTrue();
        }

        [Fact]
        public void PropertyType_ListTypeNullable()
        {
            // Arrange
            var type = typeof(FooPropertyList);
            var properties = type.GetProperties();

            // Act
            // 3. Property - public List<Foo?> Lst3 { get; set; } = new();
            // The list's element type ('Foo') is nullable.
            var isNullable = properties[2].IsNullable();

            // Assert
            isNullable.Should().BeFalse();
        }

        [Theory]
        [InlineData(typeof(FooFieldInt))]       // 1. Field-Variable - public int value1 = 1;
        [InlineData(typeof(FooFieldClass))]     // 1. Field-Variable - public FooChild child1 = new FooChild();
        [InlineData(typeof(FooFieldList))]      // 1. Field-Variable - public List<FooChild> child3 = null;
        public void Should_return_false_for_not_nullable_fields(Type type)
        {
            // Arrange
            var fields = type.GetFields();

            // Act
            var isNullable = fields[0].IsNullable();

            // Assert
            isNullable.Should().BeFalse();
        }

        [Theory]
        [InlineData(typeof(FooFieldInt))]       // 2. Field-Variable - public int? value2 = null;
        [InlineData(typeof(FooFieldClass))]     // 2. Field-Variable - public FooChild? child2 = null;
        [InlineData(typeof(FooFieldList))]      // 2. Field-Variable - public List<FooChild>? child4 = null;
        public void Should_return_true_for_nullable_fields(Type type)
        {
            // Arrange
            var fields = type.GetFields();

            // Act
            var isNullable = fields[1].IsNullable();

            // Assert
            isNullable.Should().BeTrue();
        }

        [Theory]
        [InlineData(nameof(FooParameterFunction.CheckBarIntParam))]        // Function - public static void CheckBarIntParam(int index)
        [InlineData(nameof(FooParameterFunction.CheckBarCharArrayParam))]  // Function - public static void CheckBarCharArrayParam(char[] input)
        [InlineData(nameof(FooParameterFunction.GetChildsListParam))]      // Function - public static List<FooChild>? GetChildsListParam(List<FooNode> nodes)
        public void Should_return_false_for_nullable_parameter(string functionName)
        {
            // Arrange
            var type = typeof(FooParameterFunction);
            var method = type.GetMethod(functionName)!;
            var parameters = method.GetParameters();

            // Act
            var isNullable = parameters[0].IsNullable();

            isNullable.Should().BeFalse();
        }

        [Theory]
        [InlineData(nameof(FooParameterFunction.CheckBarNullableIntParam))]        // Function (int? index)
        [InlineData(nameof(FooParameterFunction.CheckBarNullableCharArrayParam))]  // Function (char[]? input)
        [InlineData(nameof(FooParameterFunction.GetChildrenNullableListParam))]    // Function (List<FooNode>? nodes)
        public void Should_return_true_for_nullable_parameter(string functionName)
        {
            // Arrange
            var type = typeof(FooParameterFunction);
            var method = type.GetMethod(functionName)!;
            var parameters = method.GetParameters();

            // Act
            var isNullable = parameters[0].IsNullable();

            // Assert
            isNullable.Should().BeTrue();
        }

#pragma warning disable CS0649 // Field will always have the default value

        private sealed class FooFieldInt
        {
            public int Value1 = 1;
            public int? Value2;
        }

        private sealed class FooFieldClass
        {
            public FooChild Child1 = new();
            public FooChild? Child2;
        }

        private sealed class FooFieldList
        {
            public List<FooChild> Child3 = [];
            public List<FooChild>? Child4;
        }

        private sealed class FooPropertyInt
        {
            public int Int1 { get; set; }
            public int? Int2 { get; set; }
        }

        private sealed class FooPropertyString
        {
            public string Str1 { get; set; } = string.Empty;
            public string? Str2 { get; set; }
        }

        private sealed class FooPropertyList
        {
            public List<FooChild> Lst1 { get; set; } = [];
            public List<FooChild>? Lst2 { get; set; }
            public List<FooChild?> Lst3 { get; set; } = [];
        }

        private sealed class FooPropertyClass
        {
            public FooNode FooNode1 { get; set; } = new();
            public FooNode? FooNode2 { get; set; }
        }

#pragma warning restore CS0649 // Field will always have the default value

        private static class FooParameterFunction
        {
            public static void CheckBarIntParam(int _)
            {
            }

            public static void CheckBarCharArrayParam(char[] _)
            {
            }

            public static List<FooChild>? GetChildsListParam(List<FooNode> nodes) => GetChildrenNullableListParam(nodes);

            public static void CheckBarNullableIntParam(int? _)
            {
            }

            public static void CheckBarNullableCharArrayParam(char[]? _)
            {
            }

            public static List<FooChild> GetChildrenNullableListParam(List<FooNode>? nodes)
            {
                var childs = new List<FooChild>();
                if (nodes?.Count > 1)
                {
                    childs.Add(new() { Value1 = nodes[0].Value1 });
                    childs.Add(new() { Value2 = nodes[1].Value2 });
                }
                return childs;
            }
        }

        private class FooChild
        {
            public int? Value1 = 0;
            public int Value2 = 2;
        }

        private sealed class FooNode : FooChild
        {
            public int? Node1 = 0;
            public int Node2 = 2;
        }

        private enum FooKind
        {
            None,
            FirstKind,
            SecondKind,
            ThirdKind,
            FourthKind
        };
    }
}
