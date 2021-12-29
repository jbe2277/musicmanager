using System;

namespace Waf.MusicManager.Applications.Services;

internal class ApplicationBusyContext : IDisposable
{
    private readonly Action<ApplicationBusyContext> disposeCallback;

    public ApplicationBusyContext(Action<ApplicationBusyContext> disposeCallback)
    {
        this.disposeCallback = disposeCallback;
    }

    public void Dispose() => disposeCallback(this);
}
