using Sdk.Client.ControlPanels.Attributes;
using System.Reflection;

namespace Sdk.Client.ControlPanels.Extensions;

internal static class TypeExtensions
{
    public static Type? GetControlPanelCategoryDescriptorType(this Type controlPanelComponentType)
    {
        var attributeType = controlPanelComponentType.GetCustomAttributes()
            .Select(customAttribute => customAttribute.GetType())
            .FirstOrDefault(customAttributeType => customAttributeType.Name == typeof(ControlPanelCategoryAttribute<>).Name);

        if (attributeType is not null)
        {
            var descriptorType = attributeType.GenericTypeArguments[0];

            return descriptorType;
        }

        return null;
    }

    public static Type? GetControlPanelGroupDescriptorType(this Type controlPanelComponentType)
    {
        var attributeType = controlPanelComponentType.GetCustomAttributes()
            .Select(customAttribute => customAttribute.GetType())
            .FirstOrDefault(customAttributeType => customAttributeType.Name == typeof(ControlPanelGroupAttribute<>).Name);

        if (attributeType is not null)
        {
            var descriptorType = attributeType.GenericTypeArguments[0];

            return descriptorType;
        }

        return null;
    }
}
