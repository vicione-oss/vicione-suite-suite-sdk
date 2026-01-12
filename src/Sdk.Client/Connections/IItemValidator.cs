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
    /// Validates the specified object and returns any validation errors.
    /// </summary>
    /// <param name="item">The object to validate.</param>
    /// <returns>
    /// A dictionary of validation errors, where each key is a field or property name,
    /// and the associated list contains one or more error messages for that key.
    /// </returns>
    IDictionary<string, List<string>> Validate(object item);
}

/// <summary>
/// Defines a strongly typed contract for validating items of type <typeparamref name="TItem"/>.
/// </summary>
/// <typeparam name="TItem">The type of item to validate.</typeparam>
/// <remarks>
/// Implement this interface to provide compile‑time type safety for validation logic.
/// It inherits from <see cref="IItemValidator"/> for use in scenarios where the type is not known at compile time.
/// </remarks>
public interface IItemValidator<in TItem> : IItemValidator
{
    /// <summary>
    /// Validates the specified item of type <typeparamref name="TItem"/> and returns any validation errors.
    /// </summary>
    /// <param name="item">The strongly typed item to validate.</param>
    /// <returns>
    /// A dictionary of validation errors, where each key is a field or property name,
    /// and the associated list contains one or more error messages for that key.
    /// </returns>
    IDictionary<string, List<string>> Validate(TItem item);
}
