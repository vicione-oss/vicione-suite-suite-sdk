using System.Linq.Expressions;
using AwesomeAssertions;
using Sdk.Client.Interfaces;

namespace Sdk.Testing.Client.Interfaces;

/// <summary>
/// Reusable assertions for types implementing <see cref="IHasChangeableProperties"/>.
/// </summary>
public sealed class IHasChangeablePropertiesTests<T>
    where T : IHasChangeableProperties, new()
{
    /// <summary>
    /// Sets a property on a fresh <typeparamref name="T"/> from <paramref name="initialValue"/> to <paramref name="value"/> and
    /// asserts whether <see cref="IHasChangeableProperties.Changed"/> names that property.
    /// </summary>
    /// <param name="propertySelector">Selects a public, settable property, e.g. <c>x =&gt; x.Name</c>.</param>
    /// <param name="initialValue">The value set before the handler is attached.</param>
    /// <param name="value">The value set while the handler listens.</param>
    /// <param name="shouldTriggerChangedEvent">Whether the last raised event is expected to name the property.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="propertySelector"/> does not select a settable property.</exception>
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
