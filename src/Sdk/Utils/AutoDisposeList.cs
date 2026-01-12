namespace Sdk.Utils;

public sealed class AutoDisposeList<T> : List<T>, IDisposable where T : IDisposable
{
    public void Dispose()
    {
        foreach (var obj in this)
            obj.Dispose();
    }
}
