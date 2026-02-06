using AwesomeAssertions;
using Sdk.Client.Contracts;
using Sdk.Client.Factories;
using TestModule.Client;
using Xunit;

namespace Sdk.Client.Tests.Factories;

public sealed class ResourceFactoryTests
{
    public sealed class CreateGlobalScript
    {
        [Fact]
        public void Should_create_script_resource_with_expected_url_and_properties()
        {
            // Arrange
            const string Bundle = "main";
            const string Filename = "test.js";

            // Act
            var result = ResourceFactory.CreateGlobalScript(Bundle, Filename);

            // Assert
            result.ResourceType.Should().Be(ResourceType.Script);
            result.Bundle.Should().Be(Bundle);
            result.Url!.OriginalString.Should().Contain("/js/" + Filename);
            result.Declaration.Should().Be(ResourceDeclaration.Global);
        }
    }

    public sealed class CreateGlobalStylesheet
    {
        [Fact]
        public void Should_create_stylesheet_resource_with_expected_id_and_url()
        {
            // Arrange
            const string Filename = "styles.css";

            // Act
            var result = ResourceFactory.CreateGlobalStylesheet("bundle", Filename);

            // Assert
            result.ResourceType.Should().Be(ResourceType.Stylesheet);
            result.Id.Should().StartWith("i"); // hashed id
            result.Url!.OriginalString.Should().Contain("/css/" + Filename);
            result.Declaration.Should().Be(ResourceDeclaration.Global);
        }
    }

    public sealed class CreateModuleScript
    {
        [Fact]
        public void Should_create_local_module_script_by_default()
        {
            // Arrange
            const string File = "index.js";

            // Act
            var result = ResourceFactory.CreateModuleScript<TestClientModule>(null, File);

            // Assert
            result.ResourceType.Should().Be(ResourceType.Script);
            result.Url!.OriginalString.Should().Contain("_content");
            result.Declaration.Should().Be(ResourceDeclaration.Local);
        }

        [Fact]
        public void Should_create_global_module_script_if_forced()
        {
            // Arrange
            const string File = "index.js";

            // Act
            var result = ResourceFactory.CreateModuleScript<TestClientModule>(null, File, forceGlobal: true);

            // Assert
            result.Declaration.Should().Be(ResourceDeclaration.Global);
        }
    }

    public sealed class CreateModuleStylesheet
    {
        [Fact]
        public void Should_create_stylesheet_with_unique_id()
        {
            // Arrange
            const string File = "styles.css";

            // Act
            var result = ResourceFactory.CreateModuleStylesheet<TestClientModule>(null, File);

            // Assert
            result.ResourceType.Should().Be(ResourceType.Stylesheet);
            result.Id.Should().NotBeNull();
            result.Url!.OriginalString.Should().Contain("_content");
            result.Declaration.Should().Be(ResourceDeclaration.Local);
        }
    }

    public sealed class CreateScript
    {
        [Fact]
        public void Should_create_local_script_with_given_url()
        {
            // Arrange
            const string Url = "/static/script.js";

            // Act
            var result = ResourceFactory.CreateScript("bundle", Url);

            // Assert
            result.ResourceType.Should().Be(ResourceType.Script);
            result.Url.Should().Be(new Uri(Url, UriKind.Relative));
            result.Declaration.Should().Be(ResourceDeclaration.Local);
        }
    }

    public sealed class CreateStylesheet
    {
        [Fact]
        public void Should_create_local_stylesheet_with_given_url_and_hashed_id()
        {
            // Arrange
            const string Url = "/static/style.css";

            // Act
            var result = ResourceFactory.CreateStylesheet(null, Url);

            // Assert
            result.ResourceType.Should().Be(ResourceType.Stylesheet);
            result.Id.Should().StartWith("i");
            result.Url.Should().Be(new Uri(Url, UriKind.Relative));
            result.Declaration.Should().Be(ResourceDeclaration.Local);
        }
    }

    public sealed class CreateComponentScript
    {
        [Fact]
        public void Should_create_component_script_with_expected_url()
        {
            // Arrange
            const string File = "widget.js";
            var assembly = typeof(TestClientModule).Assembly;

            // Act
            var result = ResourceFactory.CreateComponentScript(null, File, assembly);

            // Assert
            result.ResourceType.Should().Be(ResourceType.Script);
            result.Url!.OriginalString.Should().Contain("_content");
            result.Url!.OriginalString.Should().Contain(File);
        }
    }

    public sealed class CreateComponentStylesheet
    {
        [Fact]
        public void Should_create_component_stylesheet_with_expected_url_and_id()
        {
            // Arrange
            const string File = "widget.css";
            var assembly = typeof(TestClientModule).Assembly;

            // Act
            var result = ResourceFactory.CreateComponentStylesheet(null, File, assembly);

            // Assert
            result.ResourceType.Should().Be(ResourceType.Stylesheet);
            result.Url!.OriginalString.Should().Contain("_content");
            result.Url!.OriginalString.Should().Contain(File);
            result.Id.Should().StartWith("i");
        }
    }
}
