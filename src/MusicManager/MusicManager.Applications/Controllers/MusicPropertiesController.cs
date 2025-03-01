using System.ComponentModel.Composition;
using Waf.MusicManager.Applications.Properties;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Playlists;

namespace Waf.MusicManager.Applications.Controllers;

[Export, Export(typeof(IMusicPropertiesService))]
internal class MusicPropertiesController : IMusicPropertiesService
{
    private readonly IShellService shellService;
    private readonly IMusicFileContext musicFileContext;
    private readonly ISelectionService selectionService;
    private readonly Lazy<MusicPropertiesViewModel> musicPropertiesViewModel;
    private readonly ChangeTrackerService changeTrackerService;
    private readonly HashSet<MusicFile> musicFilesToSaveAfterPlaying;
    private TaskCompletionSource<object?>? allFilesSavedCompletion;
        
    [ImportingConstructor]
    public MusicPropertiesController(IShellService shellService, IMusicFileContext musicFileContext, ISelectionService selectionService, Lazy<MusicPropertiesViewModel> musicPropertiesViewModel)
    {
        this.shellService = shellService;
        this.musicFileContext = musicFileContext;
        this.selectionService = selectionService;
        this.musicPropertiesViewModel = musicPropertiesViewModel;
        changeTrackerService = new();
        musicFilesToSaveAfterPlaying = [];
    }

    public PlaylistManager PlaylistManager { get; set; } = null!;

    private MusicPropertiesViewModel MusicPropertiesViewModel => musicPropertiesViewModel.Value;

    public void Initialize()
    {
        ServiceLocator.RegisterInstance<IChangeTrackerService>(changeTrackerService);

        PlaylistManager.PropertyChanged += PlaylistManagerPropertyChanged;
        selectionService.SelectedMusicFiles.CollectionChanged += SelectedMusicFilesCollectionChanged;
        shellService.MusicPropertiesView = MusicPropertiesViewModel.View;
    }

    public void Shutdown()
    {
        var task = SaveDirtyFilesAsync();
        shellService.AddTaskToCompleteBeforeShutdown(task);

        if (musicFilesToSaveAfterPlaying.Any())
        {
            allFilesSavedCompletion = new();
            shellService.AddTaskToCompleteBeforeShutdown(allFilesSavedCompletion.Task);
        }
    }

    public void SelectMusicFiles(IReadOnlyList<MusicFile> musicFiles)
    {
        SaveDirtyFilesAsync().NoWait();
            
        if (musicFiles.Count <= 1)
        {
            MusicPropertiesViewModel.MusicFile = musicFiles.FirstOrDefault();
        }
        else
        {
            MusicPropertiesViewModel.MusicFile = musicFileContext.CreateFromMultiple(musicFiles);
        }
    }

    private async Task SaveDirtyFilesAsync()
    {
        if (MusicPropertiesViewModel.MusicFile != null) musicFileContext.ApplyChanges(MusicPropertiesViewModel.MusicFile);

        var tasks = changeTrackerService.GetEntitiesWithChanges().Cast<MusicMetadata>().Select(x => SaveChangesAsync((MusicFile)x.Parent));
        await Task.WhenAll(tasks);
    }

    private async Task SaveMusicFilesToSaveAfterPlayingAsync()
    {
        var tasks = musicFilesToSaveAfterPlaying.ToArray().Select(SaveChangesAsync);
        await Task.WhenAll(tasks);
    }

    private async void PlaylistManagerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PlaylistManager.CurrentItem)) await SaveMusicFilesToSaveAfterPlayingAsync();
    }

    private async Task SaveChangesAsync(MusicFile musicFile)
    {
        if (musicFile == null) return;
            
        IReadOnlyList<MusicFile> allFilesToSave;
        if (musicFile.SharedMusicFiles.Any())
        {
            allFilesToSave = musicFile.SharedMusicFiles;
        }
        else
        {
            allFilesToSave = [ musicFile ];
        }

        // Filter out the music file that is currently playing
        var playingMusicFile = PlaylistManager.CurrentItem?.MusicFile;
        var filesToSave = playingMusicFile is null ? allFilesToSave : allFilesToSave.Except([playingMusicFile]).ToArray();
        if (playingMusicFile != null && allFilesToSave.Contains(playingMusicFile)) musicFilesToSaveAfterPlaying.Add(playingMusicFile);

        if (!filesToSave.Any()) return;
        var tasks = filesToSave.Select(SaveChangesCoreAsync).ToArray();
        try
        {
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            Log.Default.Error(ex, "SaveChangesAsync");
            if (filesToSave.Count == 1)
            {
                shellService.ShowError(ex, Resources.CouldNotSaveFile, filesToSave[0].FileName);
            }
            else
            {
                shellService.ShowError(ex, Resources.CouldNotSaveFiles);
            }
        }
        finally
        {
            RemoveMusicFilesToSaveAfterPlaying(filesToSave);
        }
    }

    private async Task SaveChangesCoreAsync(MusicFile musicFile)
    {
        if (!musicFile.IsMetadataLoaded) return;
        try
        {
            changeTrackerService.RemoveEntity(musicFile.Metadata);
            await musicFileContext.SaveChangesAsync(musicFile);
        }
        catch (Exception)
        {
            changeTrackerService.EntityHasChanges(musicFile.Metadata);
            throw;
        }
    }

    private void RemoveMusicFilesToSaveAfterPlaying(IEnumerable<MusicFile> musicFiles)
    {
        foreach (var x in musicFiles) musicFilesToSaveAfterPlaying.Remove(x);
            
        if (allFilesSavedCompletion != null && !musicFilesToSaveAfterPlaying.Any())
        {
            allFilesSavedCompletion.TrySetResult(null);
        }
    }

    private void SelectedMusicFilesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SelectMusicFiles(selectionService.SelectedMusicFiles.Select(x => x.MusicFile).ToArray());
    }
}
