using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.Applications;
using System.Waf.UnitTesting;
using Test.MusicManager.Applications.Data;
using Test.MusicManager.Applications.Services;
using Test.MusicManager.Applications.UnitTesting;
using Test.MusicManager.Applications.Views;
using Waf.MusicManager.Applications.Controllers;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;

namespace Test.MusicManager.Applications.Controllers
{
    [TestClass]
    public class ManagerControllerTest : ApplicationsTest
    {
        private const string subDirectory = "Files";
        private static readonly string subDirectoryPath = Path.Combine(Environment.CurrentDirectory, subDirectory);
        
        private ManagerController controller = null!;
        private ShellService shellService = null!;
        private IManagerStatusService managerStatusService = null!;
        private SelectionService selectionService = null!;
        private ManagerViewModel viewModel = null!;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            controller = Container.GetExportedValue<ManagerController>();
            controller.Initialize();

            shellService = Container.GetExportedValue<ShellService>();
            selectionService = Container.GetExportedValue<SelectionService>();
            managerStatusService = Container.GetExportedValue<IManagerStatusService>();
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
        public void NavigateFolderBrowser()
        {
            Assert.IsFalse(viewModel.NavigateDirectoryUpCommand.CanExecute(null));
            Assert.IsTrue(viewModel.FolderBrowser.SubDirectories.Any(x => @"C:\".Equals(x.Path, StringComparison.OrdinalIgnoreCase)));

            AssertHelper.CanExecuteChangedEvent(viewModel.NavigateDirectoryUpCommand, () => viewModel.FolderBrowser.CurrentPath = subDirectoryPath);
            AssertHelper.PropertyChangedEvent(viewModel.FolderBrowser, x => x.SubDirectories, () => viewModel.NavigateDirectoryUpCommand.Execute(null));
            Assert.AreEqual(Environment.CurrentDirectory, viewModel.FolderBrowser.CurrentPath);

            AssertHelper.ExpectedException<InvalidOperationException>(() => viewModel.NavigateToSelectedSubDirectoryCommand.Execute(null));
            
            viewModel.FolderBrowser.SelectedSubDirectory = viewModel.FolderBrowser.SubDirectories.First(x => subDirectoryPath.Equals(x.Path, StringComparison.OrdinalIgnoreCase));
            viewModel.NavigateToSelectedSubDirectoryCommand.Execute(null);
            Assert.AreEqual(subDirectoryPath, viewModel.FolderBrowser.CurrentPath);

            var environmentService = Container.GetExportedValue<MockEnvironmentService>();
            environmentService.MusicPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            environmentService.PublicMusicPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic);

            viewModel.NavigateHomeCommand.Execute(null);
            Assert.AreEqual(environmentService.MusicPath, viewModel.FolderBrowser.CurrentPath);

            viewModel.NavigatePublicHomeCommand.Execute(null);
            Assert.AreEqual(environmentService.PublicMusicPath, viewModel.FolderBrowser.CurrentPath);
        }

        [TestMethod]
        public void UpdateMusicFilesAndSearchFilter()
        {
            Context.WaitFor(() => managerStatusService.UpdatingFilesList == false, TimeSpan.FromSeconds(5));
            
            AssertHelper.PropertyChangedEvent(managerStatusService, x => x.UpdatingFilesList, () => viewModel.FolderBrowser.CurrentPath = subDirectoryPath);
            Assert.IsTrue(managerStatusService.UpdatingFilesList);
            Assert.AreEqual(-1, managerStatusService.TotalFilesCount);

            // Music files are updated asynchronously
            Assert.IsFalse(selectionService.MusicFiles.Any());
            Context.WaitFor(() => managerStatusService.UpdatingFilesList == false, TimeSpan.FromSeconds(5));
            Assert.IsTrue(selectionService.MusicFiles.Any(x => x.MusicFile.FileName!.EndsWith("TestMP3.mp3", StringComparison.OrdinalIgnoreCase)));
            Assert.IsTrue(managerStatusService.TotalFilesCount > 0);

            // Move a directory up.
            Assert.IsFalse(managerStatusService.UpdatingFilesList);
            viewModel.NavigateDirectoryUpCommand.Execute(null);
            Context.WaitFor(() => managerStatusService.UpdatingFilesList == false, TimeSpan.FromSeconds(5));
            Assert.IsFalse(selectionService.MusicFiles.Any(x => x.MusicFile.FileName!.EndsWith("TestMP3.mp3", StringComparison.OrdinalIgnoreCase)));

            // Load recursive all subdirectories
            Assert.IsFalse(managerStatusService.UpdatingFilesList);
            viewModel.LoadRecursiveCommand.Execute(null);
            Context.WaitFor(() => managerStatusService.UpdatingFilesList == false, TimeSpan.FromSeconds(5));
            Assert.IsTrue(selectionService.MusicFiles.Any(x => x.MusicFile.FileName!.EndsWith("TestMP3.mp3", StringComparison.OrdinalIgnoreCase)));

            // Search for TestMp3
            Assert.IsFalse(managerStatusService.UpdatingFilesList);
            viewModel.SearchFilter.UserSearchFilter = "TestMp3";
            Context.WaitFor(() => managerStatusService.UpdatingFilesList == false, TimeSpan.FromSeconds(5));
            Assert.IsNotNull(selectionService.MusicFiles.Single(x => x.MusicFile.FileName!.EndsWith("TestMP3.mp3", StringComparison.OrdinalIgnoreCase)));

            // Search for NoFilesToFind
            Assert.IsFalse(managerStatusService.UpdatingFilesList);
            viewModel.SearchFilter.UserSearchFilter = "NoFilesToFind";
            Context.WaitFor(() => managerStatusService.UpdatingFilesList == false, TimeSpan.FromSeconds(5));
            Assert.IsFalse(selectionService.MusicFiles.Any());
        }

        [TestMethod]
        public void FileSystemWatcherService()
        {
            var fileSystemWatcherService = Container.GetExportedValue<MockFileSystemWatcherService>();
            viewModel.FolderBrowser.CurrentPath = subDirectoryPath;
            Assert.IsTrue(subDirectoryPath.Equals(fileSystemWatcherService.Path, StringComparison.OrdinalIgnoreCase));
            Assert.IsTrue(fileSystemWatcherService.EnableRaisingEvents);

            viewModel.LoadRecursiveCommand.Execute(null);
            Assert.IsFalse(fileSystemWatcherService.EnableRaisingEvents);

            viewModel.FolderBrowser.CurrentPath = "";
            viewModel.FolderBrowser.CurrentPath = subDirectoryPath;
            Context.WaitFor(() => managerStatusService.UpdatingFilesList == false, TimeSpan.FromSeconds(5));
            Assert.IsTrue(selectionService.MusicFiles.Any(x => x.MusicFile.FileName!.EndsWith("TestMP3.mp3", StringComparison.OrdinalIgnoreCase)));

            fileSystemWatcherService.RaiseDeleted(new FileSystemEventArgs(WatcherChangeTypes.Deleted, subDirectoryPath, "testmp3.mp3"));
            Assert.IsFalse(selectionService.MusicFiles.Any(x => x.MusicFile.FileName!.EndsWith("TestMP3.mp3", StringComparison.OrdinalIgnoreCase)));
            Assert.IsTrue(selectionService.MusicFiles.Any());

            fileSystemWatcherService.RaiseCreated(new FileSystemEventArgs(WatcherChangeTypes.Created, subDirectoryPath, "aaa.mp3"));
            Assert.IsTrue(Path.Combine(subDirectoryPath, "aaa.mp3").Equals(selectionService.MusicFiles.First().MusicFile.FileName, StringComparison.OrdinalIgnoreCase));

            fileSystemWatcherService.RaiseRenamed(new RenamedEventArgs(WatcherChangeTypes.Renamed, subDirectoryPath, "zzz.mp3", "aaa.mp3"));
            Assert.IsTrue(Path.Combine(subDirectoryPath, "zzz.mp3").Equals(selectionService.MusicFiles.Last().MusicFile.FileName, StringComparison.OrdinalIgnoreCase));
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
}
