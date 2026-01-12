namespace Sdk.Messaging;

/// <summary>
/// Marks a message or event to be automatically forwarded to the user interface.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class ForwardToUIAttribute : Attribute;
