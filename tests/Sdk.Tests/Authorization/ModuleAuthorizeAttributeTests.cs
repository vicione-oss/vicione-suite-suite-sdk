using Sdk.Authorization;
using Sdk.Authorization.Extensions;

namespace Sdk.Tests.Authorization;

public class ModuleAuthorizeAttributeTests
{
    public class CtorCheck
    {
        private const string ModuleName = "myModule";

        [Fact]
        public void AcceptanceOneParameter()
        {
            var faa = new ModuleAuthorizeAttribute(ModuleName);
            var accessLevelRequirement = faa.GetAccessLevelRequirement();

            Assert.NotNull(accessLevelRequirement);
            Assert.Equal(ModuleName, accessLevelRequirement.ModuleId);
            Assert.Equal(AccessLevel.Partial, accessLevelRequirement.MinimumAccessLevel);
        }

        [Fact]
        public void AcceptanceTwoParameters()
        {
            var faa = new ModuleAuthorizeAttribute(ModuleName, AccessLevel.Partial);

            var accessLevelRequirement = faa.GetAccessLevelRequirement();

            Assert.NotNull(accessLevelRequirement);
            Assert.Equal(ModuleName, accessLevelRequirement.ModuleId);
            Assert.Equal(AccessLevel.Partial, accessLevelRequirement.MinimumAccessLevel);
        }
    }
}
