using System.Collections.Immutable;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Sdk.Backend.Messaging;
using Xunit;

namespace Sdk.Analyzers.Tests;

public class MustDeclareAnalyzerTests
{
    private const string DiagnosticId = "VOSDK001";

    private static async Task<ImmutableArray<Diagnostic>> RunAnalyzer(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source, cancellationToken: TestContext.Current.CancellationToken);

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>()
            .ToList();

        // Sdk messaging is not necessarily loaded yet, so it is referenced explicitly.
        references.Add(MetadataReference.CreateFromFile(typeof(IActivityArgument).Assembly.Location));

        var compilation = CSharpCompilation.Create(
            assemblyName: "TestAssembly",
            syntaxTrees: [syntaxTree],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var compilationWithAnalyzers = compilation.WithAnalyzers([new MustDeclareAnalyzer()]);

        return await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task Should_report_error_when_concrete_activity_argument_has_no_attribute()
    {
        // Arrange
        const string Source = """
            using Sdk.Backend.Messaging;

            namespace Test.Messaging;

            public sealed record MyActivityArgument : IActivityArgument;
            """;

        // Act
        var diagnostics = await RunAnalyzer(Source);

        // Assert
        diagnostics.Should().ContainSingle()
            .Which.Id.Should().Be(DiagnosticId);
    }

    [Fact]
    public async Task Should_report_error_when_instance_dependent_activity_argument_has_no_attribute()
    {
        // Arrange
        const string Source = """
            using Sdk.Backend.Messaging;

            namespace Test.Messaging;

            public sealed record MyActivityArgument : IInstanceDependentActivityArgument;
            """;

        // Act
        var diagnostics = await RunAnalyzer(Source);

        // Assert
        diagnostics.Should().ContainSingle()
            .Which.Id.Should().Be(DiagnosticId);
    }

    [Fact]
    public async Task Should_report_error_when_attribute_is_only_on_implemented_interface()
    {
        // Arrange
        // The runtime attribute lookup does not consider implemented interfaces,
        // so an attribute there does not prevent the runtime exception.
        const string Source = """
            using Sdk.Backend.Messaging;
            using Sdk.Messaging;

            namespace Test.Messaging;

            [MessageEndpoint("MyEndpoint")]
            public interface IMyActivityArgument : IActivityArgument;

            public sealed record MyActivityArgument : IMyActivityArgument;
            """;

        // Act
        var diagnostics = await RunAnalyzer(Source);

        // Assert
        diagnostics.Should().ContainSingle()
            .Which.Id.Should().Be(DiagnosticId);
    }

    [Fact]
    public async Task Should_not_report_when_attribute_is_present()
    {
        // Arrange
        const string Source = """
            using Sdk.Backend.Messaging;
            using Sdk.Messaging;

            namespace Test.Messaging;

            [MessageEndpoint("MyEndpoint")]
            public sealed record MyActivityArgument : IActivityArgument;
            """;

        // Act
        var diagnostics = await RunAnalyzer(Source);

        // Assert
        diagnostics.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_not_report_when_attribute_is_inherited_from_base_class()
    {
        // Arrange
        const string Source = """
            using Sdk.Backend.Messaging;
            using Sdk.Messaging;

            namespace Test.Messaging;

            [MessageEndpoint("MyEndpoint")]
            public abstract record ActivityArgumentBase : IActivityArgument;

            public sealed record MyActivityArgument : ActivityArgumentBase;
            """;

        // Act
        var diagnostics = await RunAnalyzer(Source);

        // Assert
        diagnostics.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_not_report_abstract_classes_and_interfaces_without_attribute()
    {
        // Arrange
        const string Source = """
            using Sdk.Backend.Messaging;

            namespace Test.Messaging;

            public interface IMyActivityArgument : IActivityArgument;

            public abstract record ActivityArgumentBase : IActivityArgument;
            """;

        // Act
        var diagnostics = await RunAnalyzer(Source);

        // Assert
        diagnostics.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_not_report_types_that_are_no_activity_arguments()
    {
        // Arrange
        const string Source = """
            using Sdk.Messaging;

            namespace Test.Messaging;

            public sealed record MyCommand : ICommand;

            public sealed record MyPlainRecord;
            """;

        // Act
        var diagnostics = await RunAnalyzer(Source);

        // Assert
        diagnostics.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_report_error_when_custom_required_attribute_is_missing()
    {
        // Arrange
        const string Source = """
            using System;
            using Sdk.Backend.Diagnostics;

            namespace Test.Diagnostics;

            public sealed class MarkerAttribute : Attribute;

            [MustDeclare(typeof(MarkerAttribute))]
            public interface IMarked;

            public sealed class Unmarked : IMarked;
            """;

        // Act
        var diagnostics = await RunAnalyzer(Source);

        // Assert
        diagnostics.Should().ContainSingle()
            .Which.Id.Should().Be(DiagnosticId);
    }

    [Fact]
    public async Task Should_not_report_when_custom_required_attribute_is_present()
    {
        // Arrange
        const string Source = """
            using System;
            using Sdk.Backend.Diagnostics;

            namespace Test.Diagnostics;

            public sealed class MarkerAttribute : Attribute;

            [MustDeclare(typeof(MarkerAttribute))]
            public interface IMarked;

            [Marker]
            public sealed class Marked : IMarked;
            """;

        // Act
        var diagnostics = await RunAnalyzer(Source);

        // Assert
        diagnostics.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_not_report_when_attribute_is_a_subtype_of_the_required_attribute()
    {
        // Arrange
        const string Source = """
            using Sdk.Backend.Messaging;
            using Sdk.Messaging;

            namespace Test.Messaging;

            public sealed class SpecificEndpointAttribute() : MessageEndpointAttribute("MyEndpoint");

            [SpecificEndpoint]
            public sealed record MyActivityArgument : IActivityArgument;
            """;

        // Act
        var diagnostics = await RunAnalyzer(Source);

        // Assert
        diagnostics.Should().BeEmpty();
    }
}
