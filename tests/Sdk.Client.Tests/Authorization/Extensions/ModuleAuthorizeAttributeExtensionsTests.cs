using AwesomeAssertions;
using Sdk.Authorization;
using Sdk.Authorization.Extensions;
using Xunit;

namespace Sdk.Client.Tests.Authorization.Extensions;

public sealed class ModuleAuthorizeAttributeExtensionsTests
{
    public class GetAccessLevelRequirement
    {
        public static readonly TheoryData<string> AccessLevelNames = [.. Enum.GetNames<AccessLevel>()];

        [Theory]
        [MemberData(nameof(AccessLevelNames))]
        public void Should_return_access_level_requirement(string accessLevelName)
        {
            // Arrange
            const string ModuleId = "FooBar";
            var accessLevelTyped = Enum.Parse<AccessLevel>(accessLevelName);

            var moduleAuthorizeAttribute = new ModuleAuthorizeAttribute(ModuleId, accessLevelTyped);

            // Act
            var accessLevelRequirement = moduleAuthorizeAttribute.GetAccessLevelRequirement();

            // Assert
            var expectedAccessLevelRequirement = new AccessLevelAuthorizationRequirement(ModuleId, accessLevelTyped);

            accessLevelRequirement.Should().NotBeNull().And.BeEquivalentTo(expectedAccessLevelRequirement);
        }
    }
}
