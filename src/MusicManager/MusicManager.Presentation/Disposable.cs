namespace Waf.MusicManager.Presentation;

public abstract class Disposable : IDisposable
{
    private bool isDisposed;

    public void Dispose()
    {
        DisposeCore(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool isDisposing) { }

    private void DisposeCore(bool isDisposing)
    {
        if (Interlocked.CompareExchange(ref isDisposed, true, false) == false) Dispose(isDisposing);
    }
}
