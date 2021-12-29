using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.Applications;
using System.Waf.UnitTesting;
using Test.MusicManager.Applications.Data;
using Test.MusicManager.Applications.UnitTesting;
using Waf.MusicManager.Applications.Controllers;
using Waf.MusicManager.Applications.Properties;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Playlists;

namespace Test.MusicManager.Applications.Controllers
{
    [TestClass]
    public class PlaylistControllerTest : ApplicationsTest
    {
        private ObservableCollection<MusicFile> musicFiles = null!;
        private MockMusicFileContext musicFileContext = null!;
        private ShellService shellService = null!;
        private SelectionService selectionService = null!;
        private PlaylistController controller = null!;
        private PlaylistViewModel viewModel = null!;
        private PlaylistManager playlistManager = null!;

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
            controller = Container.GetExportedValue<PlaylistController>();
            controller.PlaylistSettings = new PlaylistSettings();
            controller.PlaylistManager = playlistManager;
            controller.Initialize();
            controller.Run();

            shellService = Container.GetExportedValue<ShellService>();
            var view = shellService.PlaylistView!;
            viewModel = ViewHelper.GetViewModel<PlaylistViewModel>((IView)view)!;
        }

        protected override void OnCleanup()
        {
            controller.Shutdown();
            base.OnCleanup();
        }
        
        [TestMethod]
        public void PlayAndRemoveSelectedTest()
        {
            // Add music files to playlist
            Assert.IsFalse(playlistManager.Items.Any());
            viewModel.InsertMusicFilesAction(0, musicFiles);
            Assert.AreEqual(2, playlistManager.Items.Count);

            // Select the first playlist item
            Assert.IsFalse(viewModel.PlaySelectedCommand.CanExecute(null));
            Assert.IsFalse(viewModel.RemoveSelectedCommand.CanExecute(null));

            AssertHelper.CanExecuteChangedEvent(viewModel.PlaySelectedCommand, () => SetSelection(viewModel, playlistManager.Items[0]));

            Assert.IsTrue(viewModel.PlaySelectedCommand.CanExecute(null));
            Assert.IsTrue(viewModel.RemoveSelectedCommand.CanExecute(null));

            // Play the selected item
            var playerService = Container.GetExportedValue<PlayerService>();
            playerService.IsPlayCommand = true;
            bool playPauseCalled = false;
            playerService.PlayPauseCommand = new DelegateCommand(() => playPauseCalled = true);

            Assert.IsNull(playlistManager.CurrentItem);
            viewModel.PlaySelectedCommand.Execute(null);
            Assert.IsTrue(playPauseCalled);
            Assert.AreEqual(playlistManager.Items[0], playlistManager.CurrentItem);

            // Remove the selected item
            var selectedItem = viewModel.SelectedPlaylistItem;
            viewModel.RemoveSelectedCommand.Execute(null);
            Assert.AreNotEqual(selectedItem, playlistManager.Items.Single());
            Assert.AreEqual(playlistManager.Items.Single(), viewModel.SelectedPlaylistItem);
            // -- simulate the WPF behavior of updating the SelectedPlaylistItems as well
            SetSelection(viewModel, viewModel.SelectedPlaylistItem!);
            
            // Play the next selected item
            viewModel.PlaySelectedCommand.Execute(null);

            // Remove the last item as well
            AssertHelper.CanExecuteChangedEvent(viewModel.RemoveSelectedCommand, () => viewModel.RemoveSelectedCommand.Execute(null));
            Assert.IsFalse(viewModel.RemoveSelectedCommand.CanExecute(null));
            Assert.IsFalse(playlistManager.Items.Any());
            Assert.IsNull(viewModel.SelectedPlaylistItem);
        }

        [TestMethod]
        public void ShowMusicPropertiesTest()
        {
            // Initialize the MusicPropertiesController
            var musicPropertiesController = Container.GetExportedValue<MusicPropertiesController>();
            musicPropertiesController.PlaylistManager = playlistManager;
            musicPropertiesController.Initialize();
            
            // Add music files to playlist
            viewModel.InsertMusicFilesAction(0, musicFiles);

            // Select the first playlist item
            SetSelection(viewModel, playlistManager.Items[0]);

            // Show music properties view of the selected item
            var musicPropertiesView = shellService.MusicPropertiesView!;
            var musicPropertiesViewModel = ViewHelper.GetViewModel<MusicPropertiesViewModel>((IView)musicPropertiesView)!;
            Assert.IsNull(musicPropertiesViewModel.MusicFile);

            bool showMusicPropertiesViewCalled = false;
            shellService.ShowMusicPropertiesViewAction = () => showMusicPropertiesViewCalled = true;
            
            viewModel.ShowMusicPropertiesCommand.Execute(null);
            Assert.IsTrue(showMusicPropertiesViewCalled);
            Assert.AreEqual(viewModel.SelectedPlaylistItem!.MusicFile, musicPropertiesViewModel.MusicFile);

            // Clear playlist
            Assert.AreEqual(2, playlistManager.Items.Count);
            viewModel.ClearListCommand.Execute(null);
            Assert.AreEqual(0, playlistManager.Items.Count);
        }

        private static void SetSelection(PlaylistViewModel viewModel, params PlaylistItem[] items)
        {
            viewModel.SelectedPlaylistItem = items.Last();
            viewModel.SelectedPlaylistItems.Clear();
            foreach (var x in items) viewModel.SelectedPlaylistItems.Add(x);
        }
    }
}
