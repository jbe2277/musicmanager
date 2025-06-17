﻿using Waf.MusicManager.Applications.Properties;
using Waf.MusicManager.Applications.Views;

namespace Waf.MusicManager.Applications.Services;

internal class ShellService : Model, IShellService
{
    private readonly Lazy<IShellView> shellView;
    private readonly List<Task> tasksToCompleteBeforeShutdown;
    private readonly List<ApplicationBusyContext> applicationBusyContext;
    private object? contentView;
    private object? musicPropertiesView;
    private object? playlistView;
    private Lazy<object>? transcodeListView;
    private object? playerView;
    private bool isApplicationBusy;
    private bool isClosingEventInitialized;
    private event CancelEventHandler? closing;

    public ShellService(Lazy<IShellView> shellView)
    {
        this.shellView = shellView;
        tasksToCompleteBeforeShutdown = [];
        applicationBusyContext = [];
    }

    public AppSettings Settings { get; set; } = null!;

    public object ShellView => shellView.Value;

    public object? ContentView
    {
        get => contentView;
        set => SetProperty(ref contentView, value);
    }

    public object? MusicPropertiesView
    {
        get => musicPropertiesView;
        set => SetProperty(ref musicPropertiesView, value);
    }

    public object? PlaylistView
    {
        get => playlistView;
        set => SetProperty(ref playlistView, value);
    }

    public Lazy<object>? TranscodingListView
    {
        get => transcodeListView;
        set => SetProperty(ref transcodeListView, value);
    }

    public object? PlayerView
    {
        get => playerView;
        set => SetProperty(ref playerView, value);
    }

    public Action<Exception, string> ShowErrorAction { get; set; } = null!;

    public Action ShowMusicPropertiesViewAction { get; set; } = null!;

    public Action ShowPlaylistViewAction { get; set; } = null!;

    public Action ShowTranscodingListViewAction { get; set; } = null!;

    public IReadOnlyCollection<Task> TasksToCompleteBeforeShutdown => tasksToCompleteBeforeShutdown;

    public bool IsApplicationBusy
    {
        get => isApplicationBusy;
        private set => SetProperty(ref isApplicationBusy, value);
    }

    public event CancelEventHandler? Closing
    {
        add
        { 
            closing += value;
            InitializeClosingEvent();
        }
        remove { closing -= value; }
    }

    public void ShowError(Exception exception, string displayMessage) => ShowErrorAction(exception, displayMessage);

    public void ShowMusicPropertiesView() => ShowMusicPropertiesViewAction();

    public void ShowPlaylistView() => ShowPlaylistViewAction();

    public void ShowTranscodingListView() => ShowTranscodingListViewAction();

    public void AddTaskToCompleteBeforeShutdown(Task task) => tasksToCompleteBeforeShutdown.Add(task);

    public IDisposable SetApplicationBusy()
    {
        var context = new ApplicationBusyContext(ApplicationBusyContextDisposeCallback);
        applicationBusyContext.Add(context);
        IsApplicationBusy = true;
        return context;
    }

    protected virtual void OnClosing(CancelEventArgs e) => closing?.Invoke(this, e);

    private void ApplicationBusyContextDisposeCallback(ApplicationBusyContext context)
    {
        applicationBusyContext.Remove(context);
        IsApplicationBusy = applicationBusyContext.Any();
    }

    private void InitializeClosingEvent()
    {
        if (isClosingEventInitialized) return;
        isClosingEventInitialized = true;
        shellView.Value.Closing += ShellViewClosing;
    }

    private void ShellViewClosing(object? sender, CancelEventArgs e) => OnClosing(e);
}
