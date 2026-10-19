namespace Sdk.Client.ControlPanels.Extensions;

internal static class NullableTypeExtensions
{
    extension(Type? type)
    {
        public T? Resolve<T>(IServiceProvider serviceProvider)
            where T : class
            => type is null ? null : serviceProvider.GetService(type) as T;
    }
}
