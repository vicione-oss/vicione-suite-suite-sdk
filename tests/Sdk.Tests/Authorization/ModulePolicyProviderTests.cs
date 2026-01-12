using AwesomeAssertions;
using Sdk.Authorization;
using TestModule.Client;

namespace Sdk.Tests.Authorization;

public sealed class ModulePolicyProviderTests
{
    public sealed class GetPolicy_Generic
    {
        [Fact]
        public void Should_return_policy_string_with_default_values()
        {
            // Arrange
            const string ExpectedPrefix = Constants.AuthorizationPolicyPrefix;

            // Act
            var result = ModulePolicyProvider.GetPolicy<TestClientModule>();

            // Assert
            result.Should().StartWith($"{ExpectedPrefix}_");
            result.Should().Contain("_Partial");
        }

        [Fact]
        public void Should_include_feature_name_if_provided()
        {
            // Arrange
            const string Feature = "MyFeature";

            // Act
            var result = ModulePolicyProvider.GetPolicy<TestClientModule>(AccessLevel.Full, Feature);

            // Assert
            result.Should().EndWith("_MyFeature");
        }
    }

    public sealed class GetPolicy_Type
    {
        [Fact]
        public void Should_return_policy_for_type()
        {
            // Arrange
            var moduleType = typeof(TestClientModule);

            // Act
            var result = ModulePolicyProvider.GetPolicy(moduleType);

            // Assert
            result.Should().Contain("_Partial");
        }

        [Fact]
        public void Should_include_custom_access_level()
        {
            // Arrange
            var moduleType = typeof(TestClientModule);

            // Act
            var result = ModulePolicyProvider.GetPolicy(moduleType, AccessLevel.Full);

            // Assert
            result.Should().EndWith("_Full");
        }
    }

    public sealed class GetPolicy_String
    {
        [Fact]
        public void Should_return_policy_for_module_id()
        {
            // Arrange
            const string ModuleId = "TestModule";

            // Act
            var result = ModulePolicyProvider.GetPolicy(ModuleId);

            // Assert
            result.Should().Be($"{Constants.AuthorizationPolicyPrefix}_{ModuleId}_Partial");
        }

        [Fact]
        public void Should_append_feature_name_to_policy()
        {
            // Arrange
            const string ModuleId = "OtherModule";

            // Act
            var result = ModulePolicyProvider.GetPolicy(ModuleId, AccessLevel.Full, "FeatureX");

            // Assert
            result.Should().Be($"{Constants.AuthorizationPolicyPrefix}_{ModuleId}_Full_FeatureX");
        }
    }
}
