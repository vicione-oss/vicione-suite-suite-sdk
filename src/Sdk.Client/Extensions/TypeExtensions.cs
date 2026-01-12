namespace Sdk.Client.Extensions;

public static class TypeExtensions
{
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
