using AwesomeAssertions;
using Sdk.Client.Contracts;
using Sdk.Client.Extensions;
using Sdk.Client.Modules;
using TestModule.Client;
using Xunit;

namespace Sdk.Client.Tests.Extensions;

public sealed class ResourceExtensionsTests
{
    public sealed class AddScript
    {
        [Theory]
        [InlineData("./js/script.js", "./js/script.js")]
        [InlineData("/js/script.js", "/js/script.js")]
        [InlineData("https://host/js/script.js?v=1", "/js/script.js")]
        public void Should_add_script_with_request_path_of_uri(string address, string expected)
        {
            // Arrange
            var list = new List<Resource>();

            // Act
            list.AddScript(null, new Uri(address, UriKind.RelativeOrAbsolute));

            // Assert
            list.Should().ContainSingle().Which.Url.Should().Be(new Uri(expected, UriKind.Relative));
        }

        [Fact]
        public void Should_add_script_from_module_asset_helper_uri()
        {
            // Arrange
            var list = new List<Resource>();
            var uri = ModuleAssetHelper.GetModuleJsUrl<TestClientModule>("script.js");

            // Act
            list.AddScript(null, uri);

            // Assert
            list.Should().ContainSingle().Which.Url.Should().Be(uri);
        }
    }

    public sealed class AddStylesheet
    {
        [Theory]
        [InlineData("./css/style.css", "./css/style.css")]
        [InlineData("/css/style.css", "/css/style.css")]
        [InlineData("https://host/css/style.css?v=1", "/css/style.css")]
        public void Should_add_stylesheet_with_request_path_of_uri(string address, string expected)
        {
            // Arrange
            var list = new List<Resource>();

            // Act
            list.AddStylesheet(null, new Uri(address, UriKind.RelativeOrAbsolute));

            // Assert
            list.Should().ContainSingle().Which.Url.Should().Be(new Uri(expected, UriKind.Relative));
        }

        [Fact]
        public void Should_add_stylesheet_from_module_asset_helper_uri()
        {
            // Arrange
            var list = new List<Resource>();
            var uri = ModuleAssetHelper.GetModuleCssUrl<TestClientModule>("style.css");

            // Act
            list.AddStylesheet(null, uri);

            // Assert
            list.Should().ContainSingle().Which.Url.Should().Be(uri);
        }
    }
}
