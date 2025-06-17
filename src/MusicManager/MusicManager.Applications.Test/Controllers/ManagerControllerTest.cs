using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.Applications;
using System.Waf.UnitTesting;
using Test.MusicManager.Applications.Services;
using Test.MusicManager.Applications.Views;
using Waf.MusicManager.Applications.Controllers;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;

namespace Test.MusicManager.Applications.Controllers;

[TestClass]
public class ManagerControllerTest : ApplicationsTest
{
    private ManagerController controller = null!;
    private ShellService shellService = null!;
    private IManagerStatusService managerStatusService = null!;
    private SelectionService selectionService = null!;
    private ManagerViewModel viewModel = null!;

    protected override void OnInitialize()
    {
        base.OnInitialize();
        controller = Get<ManagerController>();
        controller.Initialize();

        shellService = Get<ShellService>();
        selectionService = Get<SelectionService>();
        managerStatusService = Get<IManagerStatusService>();
        var view = (MockManagerView)shellService.ContentView!;
        viewModel = ViewHelper.GetViewModel<ManagerViewModel>(view)!;
    }

    protected override void OnCleanup()
    {
        controller.Shutdown();
        Assert.AreEqual(viewModel.FolderBrowser.CurrentPath, shellService.Settings.CurrentPath);
        base.OnCleanup();
    }
        
    [TestMethod]
    public void FileSystemWatcherService()
    {
        var path = @"C:\Music";
        var fileService = Get<MockFileService>();
        fileService.GetFilesStub = (dir, deep, f1, f2, c) => Task.FromResult<IReadOnlyList<string>>([ Path.Combine(path, "TestMP3.mp3"), Path.Combine(path, "Test42.mp3") ]);

        var fileSystemWatcherService = Get<MockFileSystemWatcherService>();
        viewModel.FolderBrowser.CurrentPath = path;
        Assert.IsTrue(path.Equals(fileSystemWatcherService.Path, StringComparison.OrdinalIgnoreCase));
        Assert.IsTrue(fileSystemWatcherService.EnableRaisingEvents);

        viewModel.LoadRecursiveCommand.Execute(null);
        Assert.IsFalse(fileSystemWatcherService.EnableRaisingEvents);

        viewModel.FolderBrowser.CurrentPath = "";
        viewModel.FolderBrowser.CurrentPath = path;
        Context.WaitFor(() => managerStatusService.UpdatingFilesList == false, TimeSpan.FromSeconds(5));
        Assert.IsTrue(selectionService.MusicFiles.Any(x => x.MusicFile.FileName!.EndsWith("TestMP3.mp3", StringComparison.OrdinalIgnoreCase)));

        fileSystemWatcherService.RaiseDeleted(new FileSystemEventArgs(WatcherChangeTypes.Deleted, path, "testmp3.mp3"));
        Assert.IsFalse(selectionService.MusicFiles.Any(x => x.MusicFile.FileName!.EndsWith("TestMP3.mp3", StringComparison.OrdinalIgnoreCase)));
        Assert.IsTrue(selectionService.MusicFiles.Any());

        fileSystemWatcherService.RaiseCreated(new FileSystemEventArgs(WatcherChangeTypes.Created, path, "aaa.mp3"));
        Assert.IsTrue(Path.Combine(path, "aaa.mp3").Equals(selectionService.MusicFiles.First().MusicFile.FileName, StringComparison.OrdinalIgnoreCase));

        fileSystemWatcherService.RaiseRenamed(new RenamedEventArgs(WatcherChangeTypes.Renamed, path, "zzz.mp3", "aaa.mp3"));
        Assert.IsTrue(Path.Combine(path, "zzz.mp3").Equals(selectionService.MusicFiles.Last().MusicFile.FileName, StringComparison.OrdinalIgnoreCase));
    }

    [TestMethod]
    public void ShowMusicPropertiesView()
    {
        var showMusicPropertiesViewCalled = false;
        shellService.ShowMusicPropertiesViewAction = () => showMusicPropertiesViewCalled = true;

        viewModel.ShowMusicPropertiesCommand.Execute(null);
            
        Assert.IsTrue(showMusicPropertiesViewCalled);
    }
}
