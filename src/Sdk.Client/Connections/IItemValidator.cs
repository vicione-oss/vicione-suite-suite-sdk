namespace Sdk.Client.Connections;

/// <summary>
/// Defines a non-generic contract for validating an object instance.
/// </summary>
/// <remarks>
/// This interface is primarily used when the specific item type is not known at compile time.
/// Implementations are expected to throw an exception if the given object is not a supported type.
/// </remarks>
public interface IItemValidator
{
    /// <summary>
    /// Validates the specified <paramref name="item"/> and returns any validation errors.
    /// </summary>
    /// <returns>
    /// A dictionary of validation errors, where each key is a field or property name,
    /// and the associated list contains one or more error messages for that key.
    /// </returns>
    IDictionary<string, List<string>> Validate(object item);
}

/// <summary>
/// Defines a strongly typed contract for validating items of type <typeparamref name="TItem"/>.
/// </summary>
public interface IItemValidator<in TItem> : IItemValidator
{
    /// <inheritdoc cref="IItemValidator.Validate(object)"/>
    IDictionary<string, List<string>> Validate(TItem item);
}
