namespace Sdk.Messaging;

/// <summary>
/// Marks a consumer that does not modify persistent state; such a consumer may run on every instance.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ReadOnlyConsumerAttribute : Attribute;
