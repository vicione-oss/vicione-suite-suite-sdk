namespace Sdk.Client.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Type"/>.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Returns the nearest base type with the same GUID as <paramref name="requiredBaseType"/>; comparing GUIDs lets an open
    /// generic such as <c>NotificationElementBase&lt;&gt;</c> find its closed form.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if no base type of <paramref name="type"/> matches.</exception>
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
