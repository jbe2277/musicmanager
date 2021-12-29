using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.Applications;
using System.Waf.UnitTesting;
using Test.MusicManager.Applications.Data;
using Test.MusicManager.Applications.UnitTesting;
using Test.MusicManager.Applications.Views;
using Waf.MusicManager.Applications.Controllers;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Playlists;

namespace Test.MusicManager.Applications.Controllers
{
    [TestClass]
    public class MusicPropertiesControllerTest : ApplicationsTest
    {
        private ObservableCollection<MusicFile> musicFiles = null!;
        private MockMusicFileContext musicFileContext = null!;
        private SelectionService selectionService = null!;
        private MusicPropertiesController controller = null!;
        private PlaylistManager playlistManager = null!;
        private MockMusicPropertiesView view = null!;
        private MusicPropertiesViewModel viewModel = null!;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            musicFileContext = Container.GetExportedValue<MockMusicFileContext>();
            musicFiles = new ObservableCollection<MusicFile>()
            {
               musicFileContext.Create(@"C:\Users\Public\Music\Dancefloor\Culture Beat - Serenity.wav"),
               musicFileContext.Create(@"C:\Culture Beat - Serenity - Epilog.wma"),
            };
            selectionService = Container.GetExportedValue<SelectionService>();
            selectionService.Initialize(musicFiles);

            playlistManager = new PlaylistManager();
            controller = Container.GetExportedValue<MusicPropertiesController>();
            controller.PlaylistManager = playlistManager;
            controller.Initialize();

            var shellService = Container.GetExportedValue<ShellService>();
            view = (MockMusicPropertiesView)shellService.MusicPropertiesView!;
            viewModel = ViewHelper.GetViewModel<MusicPropertiesViewModel>(view)!;
        }

        protected override void OnCleanup()
        {
            controller.Shutdown();
            base.OnCleanup();
        }

        [TestMethod]
        public void SelectMusicFilesTest()
        {
            Assert.IsNull(viewModel.MusicFile);
            
            // Select the first music file
            AssertHelper.PropertyChangedEvent(viewModel, x => x.MusicFile, () => selectionService.SelectedMusicFiles.Add(selectionService.MusicFiles[0]));
            Assert.AreEqual(musicFiles[0], viewModel.MusicFile);

            viewModel.MusicFile!.Metadata!.Rating = 33;

            // Extend the selection by adding the second music file
            
            // First the controller saves changes of the previous selected music file
            var applyChangesCalled = false;
            musicFileContext.ApplyChangesAction = mf =>
            {
                Assert.AreEqual(musicFiles[0], mf);
                applyChangesCalled = true;
            };

            var saveChangesCalled = false;
            musicFileContext.SaveChangesAsyncAction = mf =>
            {
                Assert.AreEqual(musicFiles[0], mf);
                saveChangesCalled = true;
                return Task.CompletedTask;
            };

            AssertHelper.PropertyChangedEvent(viewModel, x => x.MusicFile, () => selectionService.SelectedMusicFiles.Add(selectionService.MusicFiles[1]));
            
            Assert.IsTrue(applyChangesCalled);
            Assert.IsTrue(saveChangesCalled);
            musicFileContext.ApplyChangesAction = null;
            musicFileContext.SaveChangesAsyncAction = null;

            // Both selected music files are shown as shared music file with combined properties
            AssertHelper.SequenceEqual(musicFiles, viewModel.MusicFile.SharedMusicFiles);
        }

        [TestMethod]
        public void SaveChangesErrorTest()
        {
            selectionService.MusicFiles[0].MusicFile.Metadata!.Rating = 33;

            var exception = new InvalidOperationException("Test");
            musicFileContext.SaveChangesAsyncAction = mf =>
            {
                var tcs = new TaskCompletionSource<object>();
                tcs.SetException(exception);
                return tcs.Task;
            };

            var shellService = Container.GetExportedValue<ShellService>();
            var showErrorCalled = false;
            shellService.ShowErrorAction = (ex, msg) =>
            {
                showErrorCalled = true;
                Assert.AreEqual(exception, ex);
                Assert.IsFalse(string.IsNullOrEmpty(msg));
            };

            selectionService.SelectedMusicFiles.Add(selectionService.MusicFiles[1]);
            Assert.IsTrue(showErrorCalled);

            musicFileContext.SaveChangesAsyncAction = null;
        }

        [TestMethod]
        public void SaveChangesWhilePlayingMusicFile()
        {
            selectionService.SelectedMusicFiles.Add(selectionService.MusicFiles[0]);
            selectionService.SelectedMusicFiles.Add(selectionService.MusicFiles[1]);
            selectionService.MusicFiles[0].MusicFile.Metadata!.Rating = 33;
            selectionService.MusicFiles[1].MusicFile.Metadata!.Rating = 33;

            var saveChangesCalled = 0;
            var musicFileToSave = musicFiles[1];
            musicFileContext.SaveChangesAsyncAction = mf =>
            {
                saveChangesCalled++;
                Assert.AreEqual(musicFileToSave, mf);
                musicFileToSave.Metadata!.ClearChanges();
                return Task.CompletedTask;
            };

            playlistManager.CurrentItem = new PlaylistItem(musicFiles[0]);

            // Saves just the second MusicFile because the first one is currently playing
            selectionService.SelectedMusicFiles.Clear();
            Assert.AreEqual(1, saveChangesCalled);

            // Save first MusicFile when it is not beeing played anymore
            musicFileToSave = musicFiles[0];
            playlistManager.CurrentItem = new PlaylistItem(musicFiles[1]);
            Context.WaitFor(() => saveChangesCalled == 2, TimeSpan.FromSeconds(1));

            selectionService.MusicFiles[0].MusicFile.Metadata!.Rating = 50;
            selectionService.MusicFiles[1].MusicFile.Metadata!.Rating = 50;
            selectionService.SelectedMusicFiles.Add(selectionService.MusicFiles[0]);
            selectionService.SelectedMusicFiles.Add(selectionService.MusicFiles[1]);
            Assert.AreEqual(3, saveChangesCalled);

            // Shutdown adds two tasks: 
            //   1. save selected file
            //   2. save unsaved files because they were played until now
            selectionService.MusicFiles[0].MusicFile.Metadata!.Rating = 75;
            selectionService.MusicFiles[1].MusicFile.Metadata!.Rating = 75;
            var shellService = Container.GetExportedValue<ShellService>();
            musicFileToSave = musicFiles[0];
            controller.Shutdown();
            var tasks = shellService.TasksToCompleteBeforeShutdown.ToArray();
            Assert.AreEqual(2, tasks.Length);

            Context.Wait(tasks[0]);
            Assert.AreEqual(4, saveChangesCalled);

            musicFileToSave = musicFiles[1];
            playlistManager.CurrentItem = null;
            Context.Wait(tasks[1]);
            Assert.AreEqual(5, saveChangesCalled);
        }
    }
}
