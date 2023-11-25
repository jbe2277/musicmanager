namespace Waf.MusicManager.Applications.Services;

internal sealed class ApplicationBusyContext(Action<ApplicationBusyContext> disposeCallback) : IDisposable
{
    public void Dispose() => disposeCallback(this);
}
