using System.Reflection;
using AwesomeAssertions;
using Sdk.Client.Modules;
using TestModule.Client;
using Xunit;

namespace Sdk.Client.Tests.Modules;

public static class ModuleAssetHelperTests
{
    public sealed class GetGlobalJsPath
    {
        [Fact]
        public void Should_return_relative_js_path_when_relative_true()
        {
            // Arrange
            const string Filename = "file.js";

            // Act
            var result = ModuleAssetHelper.GetGlobalJsUrl(Filename, relative: true);

            // Assert
            result.Should().Be(new Uri($"/js/{Filename}", UriKind.Relative));
        }

        [Fact]
        public void Should_return_absolute_js_path_when_relative_false()
        {
            // Arrange
            const string Filename = "file.js";

            // Act
            var result = ModuleAssetHelper.GetGlobalJsUrl(Filename);

            // Assert
            result.Should().Be(new Uri($"./js/{Filename}", UriKind.Relative));
        }

        [Fact]
        public void Should_throw_when_filename_is_null()
            => Assert.Throws<ArgumentException>(() => ModuleAssetHelper.GetGlobalJsUrl(null!));

        [Fact]
        public void Should_throw_when_filename_is_empty()
            => Assert.Throws<ArgumentException>(() => ModuleAssetHelper.GetGlobalJsUrl(""));
    }

    public sealed class GetGlobalCssPath
    {
        [Fact]
        public void Should_return_css_path_with_expected_prefix()
        {
            // Arrange
            const string Filename = "style.css";

            // Act
            var result = ModuleAssetHelper.GetGlobalCssUrl(Filename);

            // Assert
            result.Should().Be(new Uri($"./css/{Filename}", UriKind.Relative));
        }

        [Fact]
        public void Should_throw_when_filename_is_null()
            => Assert.Throws<ArgumentException>(() => ModuleAssetHelper.GetGlobalCssUrl(null!));

        [Fact]
        public void Should_throw_when_filename_is_empty()
            => Assert.Throws<ArgumentException>(() => ModuleAssetHelper.GetGlobalCssUrl(""));
    }

    public sealed class GetModuleJsPath
    {
        [Fact]
        public void Should_return_module_js_path()
        {
            // Arrange
            const string Filename = "main.js";

            // Act
            var result = ModuleAssetHelper.GetModuleJsUrl<TestClientModule>(Filename, relative: true);

            // Assert
            result.OriginalString.Should().Contain(ModuleAssetHelper.ContentPrefix);
            result.OriginalString.Should().EndWith($"/js/{Filename}");
        }

        [Fact]
        public void Should_throw_when_filename_is_null()
            => Assert.Throws<ArgumentException>(() => ModuleAssetHelper.GetModuleJsUrl<TestClientModule>(null!));

        [Fact]
        public void Should_throw_when_filename_is_empty()
            => Assert.Throws<ArgumentException>(() => ModuleAssetHelper.GetModuleJsUrl<TestClientModule>(""));
    }

    public sealed class GetModuleCssPath
    {
        [Fact]
        public void Should_return_module_css_path()
        {
            // Arrange
            const string Filename = "style.css";

            // Act
            var result = ModuleAssetHelper.GetModuleCssUrl<TestClientModule>(Filename, relative: true);

            // Assert
            result.OriginalString.Should().Contain(ModuleAssetHelper.ContentPrefix);
            result.OriginalString.Should().EndWith($"/css/{Filename}");
        }

        [Fact]
        public void Should_throw_when_filename_is_null()
            => Assert.Throws<ArgumentException>(() => ModuleAssetHelper.GetModuleCssUrl<TestClientModule>(null!));

        [Fact]
        public void Should_throw_when_filename_is_empty()
            => Assert.Throws<ArgumentException>(() => ModuleAssetHelper.GetModuleCssUrl<TestClientModule>(""));
    }

    public sealed class GetModuleImageUrl
    {
        [Fact]
        public void Should_return_module_image_path()
        {
            // Arrange
            const string Filename = "logo.png";

            // Act
            var result = ModuleAssetHelper.GetModuleImageUrl<TestClientModule>(Filename);

            // Assert
            result.OriginalString.Should().Contain("/images/");
            result.OriginalString.Should().EndWith(Filename);
        }

        [Fact]
        public void Should_throw_when_filename_is_null()
            => Assert.Throws<ArgumentException>(() => ModuleAssetHelper.GetModuleImageUrl<TestClientModule>(null!));

        [Fact]
        public void Should_throw_when_filename_is_empty()
            => Assert.Throws<ArgumentException>(() => ModuleAssetHelper.GetModuleImageUrl<TestClientModule>(""));
    }

    public sealed class GetModuleIconUrl
    {
        [Fact]
        public void Should_return_module_icon_path()
        {
            // Arrange
            const string IconName = "icon.svg";

            // Act
            var result = ModuleAssetHelper.GetModuleIconUrl<TestClientModule>(IconName);

            // Assert
            result.OriginalString.Should().Contain("/svg/");
            result.OriginalString.Should().EndWith(IconName);
        }

        [Fact]
        public void Should_throw_when_icon_name_is_null()
            => Assert.Throws<ArgumentException>(() => ModuleAssetHelper.GetModuleIconUrl<TestClientModule>(null!));

        [Fact]
        public void Should_throw_when_icon_name_is_empty()
            => Assert.Throws<ArgumentException>(() => ModuleAssetHelper.GetModuleIconUrl<TestClientModule>(""));
    }

    public sealed class GetModuleManifestName
    {
        [Fact]
        public void Should_return_name_without_dll_suffix()
        {
            // Arrange
            var assembly = Assembly.GetAssembly(typeof(TestClientModule))!;

            // Act
            var name = ModuleAssetHelper.GetModuleManifestName(assembly);

            // Assert
            name.Should().NotEndWith(".dll");
            name.Should().Be(typeof(TestClientModule).Assembly.GetName().Name);
        }

        [Fact]
        public void Should_fallback_to_manifest_module_name_when_name_is_null()
        {
            // Arrange
            var assembly = new AssemblyWithoutName("Fake.Module.dll");

            // Act
            var result = ModuleAssetHelper.GetModuleManifestName(assembly);

            // Assert
            result.Should().Be("Fake.Module");
        }

        // Stands in for a WebAssembly assembly whose AssemblyName carries no name.
        private sealed class AssemblyWithoutName(string manifestModuleName) : Assembly
        {
            public override Module ManifestModule { get; } = new NamedModule(manifestModuleName);

            public override AssemblyName GetName() => new();
        }

        private sealed class NamedModule(string name) : Module
        {
            public override string Name { get; } = name;
        }
    }
}
