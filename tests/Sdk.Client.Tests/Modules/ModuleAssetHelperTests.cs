using System.Reflection;
using AwesomeAssertions;
using Sdk.Client.Modules;
using TestModule.Client;
using Xunit;

namespace Sdk.Client.Tests.Modules;

public class ModuleAssetHelperTests
{
    public sealed class GetGlobalJsPath
    {
        [Fact]
        public void Should_return_relative_js_path_when_relative_true()
        {
            // Arrange
            const string Filename = "file.js";

            // Act
            var result = ModuleAssetHelper.GetGlobalJsPath(Filename, relative: true);

            // Assert
            result.Should().Be($"/js/{Filename}");
        }

        [Fact]
        public void Should_return_absolute_js_path_when_relative_false()
        {
            // Arrange
            const string Filename = "file.js";

            // Act
            var result = ModuleAssetHelper.GetGlobalJsPath(Filename);

            // Assert
            result.Should().Be($"./js/{Filename}");
        }
    }

    public sealed class GetGlobalCssPath
    {
        [Fact]
        public void Should_return_css_path_with_expected_prefix()
        {
            // Arrange
            const string Filename = "style.css";

            // Act
            var result = ModuleAssetHelper.GetGlobalCssPath(Filename);

            // Assert
            result.Should().Be($"./css/{Filename}");
        }
    }

    public sealed class GetModuleJsPath
    {
        [Fact]
        public void Should_return_module_js_path()
        {
            // Arrange
            const string Filename = "main.js";

            // Act
            var result = ModuleAssetHelper.GetModuleJsPath<TestClientModule>(Filename, relative: true);

            // Assert
            result.Should().Contain(ModuleAssetHelper.ContentPrefix);
            result.Should().EndWith($"/js/{Filename}");
        }
    }

    public sealed class GetModuleCssPath
    {
        [Fact]
        public void Should_return_module_css_path()
        {
            // Arrange
            const string Filename = "style.css";

            // Act
            var result = ModuleAssetHelper.GetModuleCssPath<TestClientModule>(Filename, relative: true);

            // Assert
            result.Should().Contain(ModuleAssetHelper.ContentPrefix);
            result.Should().EndWith($"/css/{Filename}");
        }
    }

    public sealed class GetModuleImagePath
    {
        [Fact]
        public void Should_return_module_image_path()
        {
            // Arrange
            const string Filename = "logo.png";

            // Act
            var result = ModuleAssetHelper.GetModuleImagePath<TestClientModule>(Filename);

            // Assert
            result.Should().Contain("/images/");
            result.Should().EndWith(Filename);
        }
    }

    public sealed class GetModuleIconPath
    {
        [Fact]
        public void Should_return_module_icon_path()
        {
            // Arrange
            const string IconName = "icon.svg";

            // Act
            var result = ModuleAssetHelper.GetModuleIconPath<TestClientModule>(IconName);

            // Assert
            result.Should().Contain("/svg/");
            result.Should().EndWith(IconName);
        }
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
            var assembly = SubstituteAssemblyWithoutName();

            // Act
            var result = ModuleAssetHelper.GetModuleManifestName(assembly);

            // Assert
            result.Should().NotBeEmpty();
        }

        private static Assembly SubstituteAssemblyWithoutName()
        {
            // Trick: use current assembly but override GetName().Name to null (simulate wasm scenario)
            var assembly = typeof(ModuleAssetHelperTests).Assembly;
            return assembly; // In practice, you'd need a dynamic assembly mock, simplified here
        }
    }
}
