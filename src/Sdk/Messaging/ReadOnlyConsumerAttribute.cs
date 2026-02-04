namespace Sdk.Messaging;

/// <summary>
/// Marks a message consumer as being "read-only". This represents a consumer that does not modify persistent state
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ReadOnlyConsumerAttribute : Attribute;
