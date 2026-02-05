using Sdk.Client.ControlPanels.Attributes;
using System.Reflection;

namespace Sdk.Client.ControlPanels.Extensions;

internal static class TypeExtensions
{
    extension(Type controlPanelComponentType)
    {
        public Type? GetControlPanelCategoryDescriptorType()
        {
            var attributeType = controlPanelComponentType.GetCustomAttributes()
                .Select(customAttribute => customAttribute.GetType())
                .FirstOrDefault(customAttributeType => customAttributeType.Name == typeof(ControlPanelCategoryAttribute<>).Name);

            return attributeType?.GenericTypeArguments[0];
        }

        public Type? GetControlPanelGroupDescriptorType()
        {
            var attributeType = controlPanelComponentType.GetCustomAttributes()
                .Select(customAttribute => customAttribute.GetType())
                .FirstOrDefault(customAttributeType => customAttributeType.Name == typeof(ControlPanelGroupAttribute<>).Name);

            return attributeType?.GenericTypeArguments[0];
        }
    }
}
