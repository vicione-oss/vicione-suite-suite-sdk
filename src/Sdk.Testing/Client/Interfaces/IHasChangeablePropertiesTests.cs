using System.Linq.Expressions;
using AwesomeAssertions;
using Sdk.Client.Interfaces;

namespace Sdk.Testing.Client.Interfaces;

/// <summary>
/// Provides a reusable tests for types that implement the <see cref="IHasChangeableProperties"/> interface.
/// </summary>
public sealed class IHasChangeablePropertiesTests<T>
    where T : IHasChangeableProperties, new()
{
    /// <summary>
    /// Asserts that <see cref="IHasChangeableProperties.Changed"/> is raised
    /// when a specific property is set to <paramref name="value"/>.
    /// </summary>
    public void AssertChangedEventHandlingWhenPropertyIsSet<TProperty>(Expression<Func<T, TProperty>> propertySelector,
        TProperty initialValue, TProperty value, bool shouldTriggerChangedEvent)
    {
        // Arrange
        if (propertySelector.Body is not MemberExpression memberExpression)
            throw new ArgumentException($"Expression is not a {nameof(MemberExpression)}", nameof(propertySelector));

        var propertyInfo = memberExpression.Member as PropertyInfo ??
            throw new ArgumentException("Expression must select a property", nameof(propertySelector));

        var propertyName = propertyInfo.Name;

        var propertySetter = propertyInfo.GetSetMethod() ??
            throw new ArgumentException("Property must have a set accessor", nameof(propertySelector));

        var changedTriggered = false;

        var state = new T();
        propertySetter.Invoke(state, [initialValue]);

        state.Changed += args => changedTriggered = args.PropertyNames.Contains(propertyName);

        // Act
        propertySetter.Invoke(state, [value]);

        // Assert
        changedTriggered.Should().Be(shouldTriggerChangedEvent);
    }
}
