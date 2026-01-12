namespace Sdk.Utils;

/// <summary>
/// Provides a list that automatically disposes all contained items when the list itself is disposed.
/// </summary>
public sealed class AutoDisposeList<T> : List<T>, IDisposable where T : IDisposable
{
    /// <inheritdoc/>
    public void Dispose()
    {
        foreach (var obj in this)
            obj.Dispose();
    }
}
