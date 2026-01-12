namespace Sdk.Client.Connections;

/// <summary>
/// Provides a base implementation of <see cref="IItemValidator{TItem}"/> for validating items of type <typeparamref name="TItem"/>.
/// </summary>
/// <typeparam name="TItem">
/// The type of item this validator can process.
/// </typeparam>
/// <remarks>
/// This base class manages a dictionary of validation errors keyed by a property or field name.
/// Derive from this class and override <see cref="ValidateInternal"/> to implement validation rules.
/// </remarks>
public abstract class ItemValidatorBase<TItem> : IItemValidator<TItem>
{
    private readonly Dictionary<string, List<string>> _errors = [];

    /// <summary>
    /// Adds a validation error message for a given key (usually a property name).
    /// </summary>
    /// <param name="key">The key (e.g., property name) that the error is associated with.</param>
    /// <param name="message">The validation error message.</param>
    /// <remarks>
    /// Multiple messages can be added for the same key.
    /// </remarks>
    protected void AddError(string key, string message)
    {
        if (_errors.TryGetValue(key, out var messages))
        {
            messages.Add(message);
        }
        else
        {
            _errors.Add(key, [message]);
        }
    }

    /// <summary>
    /// Validates an object instance by attempting to cast it to <typeparamref name="TItem"/>.
    /// </summary>
    /// <param name="item">The object to validate.</param>
    /// <returns>
    /// A dictionary of validation errors, keyed by property or field name.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the object is not of type <typeparamref name="TItem"/>.
    /// </exception>
    public IDictionary<string, List<string>> Validate(object item)
    {
        if (item is TItem typedItem)
            return Validate(typedItem);

        throw new InvalidOperationException($"Validator can't validate type {item.GetType().Name}");
    }

    /// <summary>
    /// Validates a strongly typed item and returns validation errors.
    /// </summary>
    /// <param name="item">The item to validate.</param>
    /// <returns>
    /// A dictionary of validation errors, keyed by property or field name.
    /// </returns>
    /// <remarks>
    /// Clears any previous errors before calling <see cref="ValidateInternal"/>.
    /// </remarks>
    public IDictionary<string, List<string>> Validate(TItem item)
    {
        _errors.Clear();
        ValidateInternal(item);
        return _errors;
    }

    /// <summary>
    /// When implemented in a derived class, performs validation logic for the given item
    /// and calls <see cref="AddError"/> for any validation failures.
    /// </summary>
    /// <param name="item">The item to validate.</param>
    protected abstract void ValidateInternal(TItem item);
}
