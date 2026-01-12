using System.Security.Claims;
using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sdk.Authorization;

namespace Sdk.Tests.Authorization;

public sealed class ModuleAuthorizationClaimParserTests
{
    [Fact]
    public void TryParse_should_succeed()
    {
        // Arrange
        const string ModuleId = "MyModule";
        const string Feature = "MyFeature";
        const AccessLevel AccessLevel = AccessLevel.Partial;

        var claim = ModuleAuthorizationClaimFactory.CreateClaim(ModuleId, AccessLevel, Feature);

        var claimParser = new ModuleAuthorizationClaimParser(Substitute.For<ILogger<ModuleAuthorizationClaimParser>>());

        // Act
        var result = claimParser.TryParse(claim, out var claimValue);

        // Assert
        result.Should().Be(true);
        claimValue.Should().NotBeNull();
        claimValue!.ModuleId.Should().Be(ModuleId);
        claimValue.AccessLevel.Should().Be(AccessLevel);
        claimValue.FeatureName.Should().Be(Feature);
    }

    [Fact]
    public void TryParse_should_fail_when_claim_has_wrong_type()
    {
        // Arrange
        var moduleAuthorizationClaim = ModuleAuthorizationClaimFactory.CreateClaim("MyModule", AccessLevel.Partial, "MyFeature");

        var claim = new Claim("WrongType", moduleAuthorizationClaim.Value, moduleAuthorizationClaim.Type);

        var claimParser = new ModuleAuthorizationClaimParser(Substitute.For<ILogger<ModuleAuthorizationClaimParser>>());

        // Act
        var result = claimParser.TryParse(claim, out _);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public void TryParse_should_fail_when_claim_has_wrong_value()
    {
        // Arrange
        var moduleAuthorizationClaim = ModuleAuthorizationClaimFactory.CreateClaim("MyModule", AccessLevel.Partial, "MyFeature");

        var claim = new Claim(moduleAuthorizationClaim.Type, "Wrong value");

        var claimParser = new ModuleAuthorizationClaimParser(Substitute.For<ILogger<ModuleAuthorizationClaimParser>>());

        // Act
        var result = claimParser.TryParse(claim, out _);

        // Assert
        result.Should().Be(false);
    }
}
