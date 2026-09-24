using System.Collections.ObjectModel;
using System.Reflection;

namespace Sdk.Extensions;

/// <summary>
/// Provides a set of static extension methods for reflection-related tasks.
/// </summary>
public static class ReflectionExtensions
{
    /// <summary>
    /// Gets all types within the specified assemblies that implement a given interface or derive from a base type.
    /// </summary>
    public static IEnumerable<TypeInfo> GetImplementingTypes<T>(this IEnumerable<Assembly> assemblies)
        => assemblies.SelectMany(GetImplementingTypes<T>);

    /// <summary>
    /// Gets all types within the specified assembly that implement a given interface or derive from a base type.
    /// </summary>
    public static IEnumerable<TypeInfo> GetImplementingTypes<T>(this Assembly assembly)
        => assembly.DefinedTypes.Where(x => x.IsAssignableTo(typeof(T)));

    /// <summary>
    /// Creates an instance of each type that implements the given interface or derives from the base type.
    /// The types must have a parameterless constructor.
    /// </summary>
    public static IEnumerable<T> GetInstances<T>(this IEnumerable<Assembly> assemblies)
    {
        foreach (var implementation in assemblies.GetImplementingTypes<T>())
        {
            var instance = CreateInstance<T>(implementation);
            if (instance is not null)
                yield return instance;
        }
    }

    /// <summary>
    /// Gets the first available instance of a type that implements the given interface or derives from the base type.
    /// </summary>
    public static T? GetFirstInstance<T>(this Assembly assembly) => assembly.GetInstances<T>().FirstOrDefault();

    /// <summary>
    /// Creates an instance of each type in the assembly that implements the given interface or derives from the base type.
    /// The types must have a parameterless constructor.
    /// </summary>
    public static IEnumerable<T> GetInstances<T>(this Assembly assembly) => new[] { assembly }.GetInstances<T>();

    /// <summary>
    /// Determines whether a property is nullable.
    /// </summary>
    // Based on https://stackoverflow.com/questions/58453972/how-to-use-net-reflection-to-check-for-nullable-reference-type
    public static bool IsNullable(this PropertyInfo property) =>
        IsNullableHelper(property.PropertyType, property.DeclaringType, property.CustomAttributes);

    /// <summary>
    /// Determines whether a field is nullable.
    /// </summary>
    public static bool IsNullable(this FieldInfo field) =>
        IsNullableHelper(field.FieldType, field.DeclaringType, field.CustomAttributes);

    /// <summary>
    /// Determines whether a parameter is nullable.
    /// </summary>
    public static bool IsNullable(this ParameterInfo parameter) =>
        IsNullableHelper(parameter.ParameterType, parameter.Member, parameter.CustomAttributes);

    private static bool IsNullableHelper(Type memberType, MemberInfo? declaringType, IEnumerable<CustomAttributeData> customAttributes)
    {
        if (memberType.IsValueType)
            return Nullable.GetUnderlyingType(memberType) is not null;

        var nullable = customAttributes
            .FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");
        if (nullable?.ConstructorArguments.Count == 1)
        {
            var attributeArgument = nullable.ConstructorArguments[0];
            if (attributeArgument.ArgumentType == typeof(byte[]))
            {
                var args = (ReadOnlyCollection<CustomAttributeTypedArgument>)attributeArgument.Value!;
                if (args.Count > 0 && args[0].ArgumentType == typeof(byte))
                {
                    return (byte)args[0].Value! == 2;
                }
            }
            else if (attributeArgument.ArgumentType == typeof(byte))
            {
                return (byte)attributeArgument.Value! == 2;
            }
        }

        for (var type = declaringType; type is not null; type = type.DeclaringType)
        {
            var context = type.CustomAttributes
                .FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute");
            if (context?.ConstructorArguments.Count == 1 &&
                context.ConstructorArguments.Count == 1 &&
                context.ConstructorArguments[0].ArgumentType == typeof(byte))
            {
                return (byte)context.ConstructorArguments[0].Value! == 2;
            }
        }
        // Couldn't find a suitable attribute
        return false;
    }

    private static T? CreateInstance<T>(Type implementation)
    {
        if (!implementation.GetTypeInfo().IsAbstract)
        {
            var obj = Activator.CreateInstance(implementation);
            if (obj is not null)
                return (T)obj;
        }
        return default;
    }

    /// <summary>
    /// Gets a user-friendly string representation of a type's name, including its generic arguments.
    /// </summary>
    public static string GetGenericTypeName(this Type? type)
    {
        if (type is null)
            return "null";

        if (!type.IsGenericType)
            return type.Name;

        // e.g. DbErrorCommand`1
        var name = type.Name[..^2];
        var typeNames = type.GetGenericArguments().Select(t => t.Name);

        // e.g. DbErrorCommand[CreatePingCommand]
        return $"{name}[{string.Join(',', typeNames)}]";
    }

    /// <summary>
    /// Retrieves the value of a specified attribute from an assembly.
    /// </summary>
    public static string GetAttributeValue<TAttr>(this Assembly assembly, Func<TAttr,
        string> resolveFunc, string defaultResult = "") where TAttr : Attribute
    {
        var attributes = assembly.GetCustomAttributes(typeof(TAttr), false);
        if (attributes.Length > 0)
            return resolveFunc((TAttr)attributes[0]);
        else
            return defaultResult;
    }
}
