namespace Sdk.Client.Services;

public sealed class RegistryChangedEventArgs<T> : EventArgs
{
    public required IRegistry<T> Sender { get; init; }
    public required IEnumerable<T> ItemsAdded { get; init; }
    public required IEnumerable<T> ItemsRemoved { get; init; }
}
