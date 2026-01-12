namespace Sdk.Messaging;

/// <summary>
/// Marks a message consumer as being "read-only".
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ReadOnlyConsumerAttribute : Attribute;
