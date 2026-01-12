using System.Text.Json;
using AwesomeAssertions;
using Sdk.Authorization;

namespace Sdk.Tests.Authorization;

public sealed class ModuleAuthorizationClaimFactoryTests
{
    public sealed class CreateClaim
    {
        [Fact]
        public void Should_create_claim_with_expected_type()
        {
            // Arrange
            const string ModuleId = "TestModule";
            const AccessLevel AccessLevel = AccessLevel.Full;
            const string FeatureName = "FeatureX";

            // Act
            var claim = ModuleAuthorizationClaimFactory.CreateClaim(ModuleId, AccessLevel, FeatureName);

            // Assert
            claim.Type.Should().Be(SuiteClaimTypes.ModuleAuthorization);
        }

        [Fact]
        public void Should_serialize_claim_value_as_expected_json()
        {
            // Arrange
            const string ModuleId = "AnotherModule";
            const AccessLevel AccessLevel = AccessLevel.Partial;
            const string FeatureName = "FeatureY";

            var expectedValue = JsonSerializer.Serialize(
                new ModuleAuthorizationClaimValue { ModuleId = ModuleId, AccessLevel = AccessLevel, FeatureName = FeatureName },
                ClaimValueJsonSerializerContext.Default.ModuleAuthorizationClaimValue);

            // Act
            var claim = ModuleAuthorizationClaimFactory.CreateClaim(ModuleId, AccessLevel, FeatureName);

            // Assert
            claim.Value.Should().Be(expectedValue);
        }

        [Theory]
        [InlineData("ModuleB", AccessLevel.Partial, "Feature2")]
        [InlineData("ModuleC", AccessLevel.Full, "Feature3")]
        public void Should_embed_correct_values_in_claim(string moduleId, AccessLevel accessLevel, string featureName)
        {
            // Arrange
            // Act
            var claim = ModuleAuthorizationClaimFactory.CreateClaim(moduleId, accessLevel, featureName);

            // Assert
            var deserialized = JsonSerializer.Deserialize(
                claim.Value,
                ClaimValueJsonSerializerContext.Default.ModuleAuthorizationClaimValue);

            deserialized!.ModuleId.Should().Be(moduleId);
            deserialized.AccessLevel.Should().Be(accessLevel);
            deserialized.FeatureName.Should().Be(featureName);
        }
    }
}

