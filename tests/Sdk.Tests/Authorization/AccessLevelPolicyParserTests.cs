using AwesomeAssertions;
using Sdk.Authorization;

namespace Sdk.Tests.Authorization;

public sealed class AccessLevelPolicyParserTests
{
    [Theory]
    [InlineData("")]
    [InlineData("module")]
    [InlineData("module_")]
    [InlineData("module_FooBar")]
    [InlineData("module_FooBar_UnparsableAccessLevel")]
    public void TryParseShouldFail(string policy)
    {
        // Arrange, Act
        var result = AccessLevelPolicyParser.TryParse(policy, out _);

        // Assert
        result.Should().Be(false);
    }

    [Theory]
    [InlineData("module_FooBar_Full", "FooBar", AccessLevel.Full, null)]
    [InlineData("module_Baz_Partial", "Baz", AccessLevel.Partial, null)]
    [InlineData("module_Baz_Partial_Feature", "Baz", AccessLevel.Partial, "Feature")]
    public void TryParseShouldSucceed(string policy, string expectedModuleId, AccessLevel expectedMinimumAccessLevel, string? expectedFeatureName)
    {
        // Arrange, Act
        var result = AccessLevelPolicyParser.TryParse(policy, out var accessLevelAuthorizationRequirement);

        // Assert
        result.Should().Be(true);

        accessLevelAuthorizationRequirement.Should().NotBeNull();
        accessLevelAuthorizationRequirement!.ModuleId.Should().Be(expectedModuleId);
        accessLevelAuthorizationRequirement.MinimumAccessLevel.Should().Be(expectedMinimumAccessLevel);
        accessLevelAuthorizationRequirement.FeatureName.Should().Be(expectedFeatureName);
    }
}
