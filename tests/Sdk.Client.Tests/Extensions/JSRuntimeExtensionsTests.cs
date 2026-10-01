using AwesomeAssertions;
using Microsoft.JSInterop;
using NSubstitute;
using Sdk.Client.Extensions;
using Sdk.Client.Modules;
using TestModule.Client;
using Xunit;

namespace Sdk.Client.Tests.Extensions;

public sealed class JSRuntimeExtensionsTests
{
    public sealed class ImportScript
    {
        [Theory]
        [InlineData("./js/script.js", "./js/script.js")]
        [InlineData("/js/script.js", "/js/script.js")]
        [InlineData("https://host/js/script.js?v=1", "/js/script.js")]
        public async Task Should_import_request_path_of_uri(string address, string expected)
        {
            // Arrange
            var jsRuntime = Substitute.For<IJSRuntime>();

            // Act
            await jsRuntime.ImportScript(new Uri(address, UriKind.RelativeOrAbsolute));

            // Assert
            await jsRuntime.Received(1).InvokeAsync<IJSObjectReference>("import", Arg.Is<object?[]?>(args => Equals(args![0], expected)));
        }

        [Fact]
        public async Task Should_import_module_asset_helper_uri()
        {
            // Arrange
            var jsRuntime = Substitute.For<IJSRuntime>();
            var uri = ModuleAssetHelper.GetModuleJsUrl<TestClientModule>("script.js");

            // Act
            var act = () => jsRuntime.ImportScript(uri);

            // Assert
            await act.Should().NotThrowAsync();
            await jsRuntime.Received(1).InvokeAsync<IJSObjectReference>("import",
                Arg.Is<object?[]?>(args => Equals(args![0], uri.OriginalString)));
        }
    }
}
