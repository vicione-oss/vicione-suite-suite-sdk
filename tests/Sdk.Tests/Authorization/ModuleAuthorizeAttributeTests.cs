using Sdk.Authorization;
using Sdk.Authorization.Extensions;

namespace Sdk.Tests.Authorization;

public sealed class ModuleAuthorizeAttributeTests
{
    public sealed class ConstructorCheck
    {
        private const string ModuleName = "myModule";

        [Fact]
        public void AcceptanceOneParameter()
        {
            var faa = new ModuleAuthorizeAttribute(ModuleName);
            var accessLevelRequirement = faa.GetAccessLevelAuthorizationRequirement();

            Assert.NotNull(accessLevelRequirement);
            Assert.Equal(ModuleName, accessLevelRequirement.ModuleId);
            Assert.Equal(AccessLevel.Partial, accessLevelRequirement.MinimumAccessLevel);
        }

        [Fact]
        public void AcceptanceTwoParameters()
        {
            var faa = new ModuleAuthorizeAttribute(ModuleName, AccessLevel.Partial);

            var accessLevelRequirement = faa.GetAccessLevelAuthorizationRequirement();

            Assert.NotNull(accessLevelRequirement);
            Assert.Equal(ModuleName, accessLevelRequirement.ModuleId);
            Assert.Equal(AccessLevel.Partial, accessLevelRequirement.MinimumAccessLevel);
        }
    }
}
