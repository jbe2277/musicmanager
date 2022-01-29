using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Waf.Applications;
using Waf.MusicManager.Applications.DataModels;
using Waf.MusicManager.Applications.Properties;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Applications.Controllers;

[Export]
internal class ManagerController
{
    private readonly IShellService shellService;
    private readonly IEnvironmentService environmentService;
    private readonly IFileService fileService;
    private readonly IMusicFileContext musicFileContext;
    private readonly SelectionService selectionService;
    private readonly ManagerStatusService managerStatusService;
    private readonly IFileSystemWatcherService fileSystemWatcherService;
    private readonly Lazy<ManagerViewModel> managerViewModel;
    private readonly ObservableCollection<MusicFile> musicFiles;
    private readonly DelegateCommand updateSubDirectoriesCommand;
    private readonly DelegateCommand navigateDirectoryUpCommand;
    private readonly DelegateCommand navigateHomeCommand;
    private readonly DelegateCommand navigatePublicHomeCommand;
    private readonly DelegateCommand loadRecursiveCommand;
    private readonly DelegateCommand navigateToSelectedSubDirectoryCommand;
    private readonly DelegateCommand showMusicPropertiesCommand;
    private readonly DelegateCommand deleteSelectedFilesCommand;
    private CancellationTokenSource? updateMusicFilesCancellation;
        
    [ImportingConstructor]
    public ManagerController(IShellService shellService, IEnvironmentService environmentService, IFileService fileService, IMusicFileContext musicFileContext, SelectionService selectionService, 
        ManagerStatusService managerStatusService, IFileSystemWatcherService fileSystemWatcherService, Lazy<ManagerViewModel> managerViewModel)
    {
        this.shellService = shellService;
        this.environmentService = environmentService;
        this.fileService = fileService;
        this.musicFileContext = musicFileContext;
        this.selectionService = selectionService;
        this.managerStatusService = managerStatusService;
        this.fileSystemWatcherService = fileSystemWatcherService;
        this.managerViewModel = managerViewModel;
        musicFiles = new ObservableCollection<MusicFile>();
        updateSubDirectoriesCommand = new DelegateCommand(UpdateSubDirectories);
        navigateDirectoryUpCommand = new DelegateCommand(NavigateDirectoryUp, CanNavigateDirectoryUp);
        navigateHomeCommand = new DelegateCommand(NavigateHome);
        navigatePublicHomeCommand = new DelegateCommand(NavigatePublicHome);
        loadRecursiveCommand = new DelegateCommand(LoadRecursive);
        navigateToSelectedSubDirectoryCommand = new DelegateCommand(NavigateToSelectedSubDirectory);
        showMusicPropertiesCommand = new DelegateCommand(ShowMusicProperties);
        deleteSelectedFilesCommand = new DelegateCommand(DeleteSelectedFiles);
    }

    private ManagerViewModel ManagerViewModel => managerViewModel.Value;

    public void Initialize()
    {
        selectionService.Initialize(musicFiles);

        fileSystemWatcherService.NotifyFilter = NotifyFilters.FileName;
        fileSystemWatcherService.Created += FileSystemWatcherServiceCreated;
        fileSystemWatcherService.Renamed += FileSystemWatcherServiceRenamed;
        fileSystemWatcherService.Deleted += FileSystemWatcherServiceDeleted;

        ManagerViewModel.UpdateSubDirectoriesCommand = updateSubDirectoriesCommand;
        ManagerViewModel.NavigateDirectoryUpCommand = navigateDirectoryUpCommand;
        ManagerViewModel.NavigateHomeCommand = navigateHomeCommand;
        ManagerViewModel.NavigatePublicHomeCommand = navigatePublicHomeCommand;
        ManagerViewModel.LoadRecursiveCommand = loadRecursiveCommand;
        ManagerViewModel.NavigateToSelectedSubDirectoryCommand = navigateToSelectedSubDirectoryCommand;
        ManagerViewModel.ShowMusicPropertiesCommand = showMusicPropertiesCommand;
        ManagerViewModel.DeleteSelectedFilesCommand = deleteSelectedFilesCommand;
        ManagerViewModel.FolderBrowser.PropertyChanged += FolderBrowserPropertyChanged;
        ManagerViewModel.SearchFilter.PropertyChanged += SearchFilterPropertyChanged;

        try
        {
            ManagerViewModel.FolderBrowser.CurrentPath = shellService.Settings.CurrentPath ?? environmentService.MusicPath;
        }
        catch (Exception)
        {
            ManagerViewModel.FolderBrowser.CurrentPath = environmentService.MusicPath;
        }
            
        shellService.ContentView = ManagerViewModel.View;
    }

    public void Shutdown() => shellService.Settings.CurrentPath = ManagerViewModel.FolderBrowser.CurrentPath;

    private bool CanNavigateDirectoryUp() => !string.IsNullOrEmpty(ManagerViewModel.FolderBrowser.CurrentPath);
        
    private void NavigateDirectoryUp()
    {
        try
        {
            using (shellService.SetApplicationBusy())
            {
                ManagerViewModel.FolderBrowser.CurrentPath = fileService.GetParentPath(ManagerViewModel.FolderBrowser.CurrentPath).GetResult() ?? "";
            }
        }
        catch (Exception)
        {
            // This can happen when we have lost the connection to a network drive.
            ManagerViewModel.FolderBrowser.CurrentPath = "";
        }
    }

    private void NavigateHome() => ManagerViewModel.FolderBrowser.CurrentPath = environmentService.MusicPath;
        
    private void NavigatePublicHome() => ManagerViewModel.FolderBrowser.CurrentPath = environmentService.PublicMusicPath;

    private void LoadRecursive() => UpdateMusicFiles(true);

    private void NavigateToSelectedSubDirectory()
    {
        if (ManagerViewModel.FolderBrowser.SelectedSubDirectory?.Path == null) throw new InvalidOperationException("SelectedSubDirectory must not be null."); 
        ManagerViewModel.FolderBrowser.CurrentPath = ManagerViewModel.FolderBrowser.SelectedSubDirectory.Path;
    }

    private void ShowMusicProperties() => shellService.ShowMusicPropertiesView();

    private void DeleteSelectedFiles()
    {
        foreach (var x in selectionService.SelectedMusicFiles) DeleteFileAsync(x.MusicFile.FileName!);
    }

    private async void DeleteFileAsync(string fileName)
    {
        try
        {
            await fileService.DeleteFile(fileName);
        }
        catch (Exception ex)
        {
            shellService.ShowError(ex, Resources.CouldNotDeleteFile, fileName);
        }
    }

    private void UpdateSubDirectories()
    {
        try
        {
            ManagerViewModel.FolderBrowser.SubDirectories = fileService.GetSubFoldersFromPath(ManagerViewModel.FolderBrowser.CurrentPath).GetResult();
        }
        catch (Exception ex)
        {
            Log.Default.Warn(ex, "UpdateSubDirectories");
        }
    }

    private async void UpdateMusicFiles(bool deep)
    {
        updateMusicFilesCancellation?.Cancel();
        var cancellation = new CancellationTokenSource();
        updateMusicFilesCancellation = cancellation;
        Log.Default.Trace("ManagerController.UpdateMusicFiles:Start");
        managerStatusService.StartUpdatingFilesList();
            
        musicFiles.Clear();
        var path = ManagerViewModel.FolderBrowser.CurrentPath;
        try
        {
            var filesCount = 0;
            if (fileService.DirectoryExists(path))
            {
                var userSearchFilter = ManagerViewModel.SearchFilter.UserSearchFilter;
                var applicationSearchFilter = ManagerViewModel.SearchFilter.ApplicationSearchFilter;
                var files = await GetFilesAsync(path, deep, userSearchFilter, applicationSearchFilter, cancellation.Token);

                filesCount = files.Count;

                var newFiles = files.Select(x => musicFileContext.Create(x)).ToArray();
                foreach (var newFile in newFiles)
                {
                    musicFiles.Add(newFile);
                }

                if (selectionService.MusicFiles.Any())
                {
                    selectionService.SelectedMusicFiles.Clear();
                    selectionService.SelectedMusicFiles.Add(selectionService.MusicFiles.First());
                }
            }
            managerStatusService.FinishUpdatingFilesList(filesCount);
        }
        catch (OperationCanceledException)
        {
            Log.Default.Trace("ManagerController.UpdateMusicFiles:Canceled");
        }
            
        if (cancellation == updateMusicFilesCancellation) updateMusicFilesCancellation = null;
            
        Log.Default.Trace("ManagerController.UpdateMusicFiles:End");
    }

    private void FolderBrowserPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(FolderBrowserDataModel.UserPath))
        {
            // This might throw an exception => shown in the Path TextBox as validation error.
            ManagerViewModel.FolderBrowser.CurrentPath = fileService.GetFolderFromPath(ManagerViewModel.FolderBrowser.UserPath).GetResult().Path!;
        }
        if (e.PropertyName == nameof(FolderBrowserDataModel.CurrentPath))
        {
            navigateDirectoryUpCommand.RaiseCanExecuteChanged();
            ManagerViewModel.FolderBrowser.UserPath = fileService.GetDisplayPath(ManagerViewModel.FolderBrowser.CurrentPath!).GetResult();
            UpdateSubDirectories();
            ManagerViewModel.SearchFilter.Clear();
            UpdateMusicFiles(false);
        }
    }

    private void SearchFilterPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(SearchFilterDataModel.UserSearchFilter) or nameof(SearchFilterDataModel.ApplicationSearchFilter))
        {
            var userSearchFilter = ManagerViewModel.SearchFilter.UserSearchFilter;
            var applicationSearchFilter = ManagerViewModel.SearchFilter.ApplicationSearchFilter;
            if (string.IsNullOrEmpty(userSearchFilter) && string.IsNullOrEmpty(applicationSearchFilter))
            {
                UpdateMusicFiles(false);  // Reset the search; behave like the user navigated into another folder.
            }
            else
            {
                UpdateMusicFiles(true);
            }   
        }
    }

    private Task<IReadOnlyList<string>> GetFilesAsync(string directory, bool deep, string userSearchFilter, string applicationSearchFilter, CancellationToken cancellation)
    {
        if (!deep)
        {
            fileSystemWatcherService.Path = directory;
            fileSystemWatcherService.EnableRaisingEvents = true;
        }
        else
        {
            fileSystemWatcherService.EnableRaisingEvents = false;
        }
            
        // It is necessary to run this in an own task => otherwise, reentrance would block the UI thread although this should not happen.
        return fileService.GetFiles(directory, deep, userSearchFilter, applicationSearchFilter, cancellation);
    }

    private void InsertMusicFile(string fileName)
    {
        if (!fileService.IsFileSupported(fileName)) return;
            
        var insertFileName = Path.GetFileName(fileName);
        int i;
        for (i = 0; i < musicFiles.Count; i++)
        {
            if (string.Compare(insertFileName, Path.GetFileName(musicFiles[i].FileName), true, CultureInfo.CurrentCulture) <= 0)
            {
                break;
            }
        }
        musicFiles.Insert(i, musicFileContext.Create(fileName));
    }

    private void RemoveMusicFile(string fileName)
    {
        var musicFileToRemove = musicFiles.FirstOrDefault(x => fileName.Equals(x.FileName, StringComparison.OrdinalIgnoreCase));
        if (musicFileToRemove != null) musicFiles.Remove(musicFileToRemove);
    }

    private void FileSystemWatcherServiceCreated(object sender, FileSystemEventArgs e) => InsertMusicFile(e.FullPath);

    private void FileSystemWatcherServiceRenamed(object sender, RenamedEventArgs e)
    {
        RemoveMusicFile(e.OldFullPath);
        InsertMusicFile(e.FullPath);   
    }

    private void FileSystemWatcherServiceDeleted(object sender, FileSystemEventArgs e) => RemoveMusicFile(e.FullPath);
}
