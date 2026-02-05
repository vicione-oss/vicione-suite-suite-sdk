using Microsoft.Extensions.DependencyInjection;
using Sdk.Testing.Backend;

namespace Sdk.Backend.Tests;

internal static class TestExtensions
{
    extension(IServiceCollection services)
    {
        public TestOptions SetupTestOptions(string moduleId = TestConstants.TestModuleId)
        {
            var options = new TestOptions
            {
                BoolValue = true,
                StringValue = "TestVal",
                IntValue = 234,
            };
            var config = new TestConfig().AddModuleWithOptions(moduleId, options);
            services.AddConfiguration(config);

            return options;
        }

        public TestOptions SetupInvalidTestOptions(string moduleId = TestConstants.TestModuleId)
        {
            var options = new TestOptions
            {
                BoolValue = true,
                StringValue = "TestValueToooooooooLong",
                IntValue = 2342,
            };
            var config = new TestConfig().AddModuleWithOptions(moduleId, options);
            services.AddConfiguration(config);

            return options;
        }
    }
}
