using AwesomeAssertions;
using AwesomeAssertions.Execution;
using AwesomeAssertions.Primitives;

namespace Sdk.Client.Tests.NotificationArea.Extensions;

internal static class StringAssertionsExtensions
{
    public static AndConstraint<StringAssertions> RepresentGuid(this StringAssertions assertions, string because = "",
        params object[] becauseArgs)
    {
        var isGuid = Guid.TryParse(assertions.Subject, out _);

        AssertionChain.GetOrCreate()
            .ForCondition(isGuid)
            .BecauseOf(because, becauseArgs)
            .FailWith("Expected {context:string} to be {0}{reason}, but found {1}.", true, isGuid);

        return new AndConstraint<StringAssertions>(assertions);
    }
}
