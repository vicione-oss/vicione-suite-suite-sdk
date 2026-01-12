namespace Sdk.Client.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Type"/>.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Recursively searches the inheritance hierarchy of a type to find a specific base type.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the specified <paramref name="requiredBaseType"/> is not found in the inheritance hierarchy of the <paramref name="type"/>.</exception>
    public static Type GetBaseTypeRecursive(this Type type, Type requiredBaseType)
    {
        var baseType = type.BaseType;
        while (baseType is not null && baseType.GUID != requiredBaseType.GUID)
        {
            baseType = baseType.BaseType;
        }

        if (baseType is not null)
            return baseType;
        else
            throw new InvalidOperationException($"Type '{type.Name}' does not inherit from '{requiredBaseType.Name}'");
    }
}
