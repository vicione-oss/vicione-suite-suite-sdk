using AwesomeAssertions;
using Sdk.Authorization;

namespace Sdk.Tests.Authorization.Extensions;

public sealed class ModuleAuthorizationClaimFactoryTests
{
    public sealed class CreateClaim
    {
        [Fact]
        public void Should_create_claim_with_expected_type()
        {
            // Arrange
            const string ModuleId = "MyModule";
            const string Feature = "MyFeature";

            // Act
            var claim = ModuleAuthorizationClaimFactory.CreateClaim(ModuleId, AccessLevel.Full, Feature);

            // Assert
            claim.Type.Should().Be(SuiteClaimTypes.ModuleAuthorization);
        }

        [Fact]
        public void Should_serialize_claim_value_correctly()
        {
            // Arrange
            const string ModuleId = "TestModule";
            const AccessLevel Access = AccessLevel.Partial;
            const string Feature = "FeatureA";

            // Act
            var claim = ModuleAuthorizationClaimFactory.CreateClaim(ModuleId, Access, Feature);

            // Assert
            claim.Value.Should().Contain(ModuleId);
            claim.Value.Should().Contain(Feature);
            claim.Value.Should().Contain($"{(int)Access}");
        }
    }
}
