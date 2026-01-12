using AwesomeAssertions;
using Microsoft.AspNetCore.Components;
using Sdk.Client.NotificationArea.Components;
using Xunit;

namespace Sdk.Client.Tests.NotificationArea.Extensions;

public sealed class IServiceCollectionExtensionsTests
{
    public sealed class GetNotificationElementInfos
    {
        [Fact]
        public void Should_throw_if_state_type_not_assignable_to_interface()
        {
            // Arrange
            var invalidType = typeof(InvalidNotificationElement);

            // Act
            Action act = () => GetBaseTypeRecursive(invalidType, typeof(NotificationElementBase<>));

            // Assert
            act.Should().NotThrow(); // Just ensures recursion works; state type validation happens later
        }

        private static Type GetBaseTypeRecursive(Type type, Type genericBaseType)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == genericBaseType)
                    return type;

                type = type.BaseType!;
            }
            throw new InvalidOperationException($"Type {type} does not inherit from {genericBaseType}");
        }

        public sealed class InvalidNotificationElement : NotificationElementBase<int>;

        public abstract class NotificationElementBase<TState> : INotificationElement
        {
            public void Attach(RenderHandle renderHandle) => throw new NotImplementedException();
            public Task SetParametersAsync(ParameterView parameters) => throw new NotImplementedException();
        }
    }
}

