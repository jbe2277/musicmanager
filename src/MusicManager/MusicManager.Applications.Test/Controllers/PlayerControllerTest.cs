using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.Applications;
using System.Waf.UnitTesting;
using Test.MusicManager.Applications.Data;
using Test.MusicManager.Applications.Services;
using Test.MusicManager.Applications.UnitTesting;
using Test.MusicManager.Applications.Views;
using Test.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Applications.Controllers;
using Waf.MusicManager.Applications.Properties;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Playlists;

namespace Test.MusicManager.Applications.Controllers;

[TestClass]
public class PlayerControllerTest : ApplicationsTest
{
    private ObservableCollection<MusicFile> musicFiles = null!;
    private ShellService shellService = null!;
    private SelectionService selectionService = null!;
    private PlayerController controller = null!;
    private MockPlayerView view = null!;
    private PlayerViewModel viewModel = null!;
    private PlaylistManager playlistManager = null!;
    private PlaylistSettings playlistSettings = null!;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        var musicFileContext = Container.GetExportedValue<MockMusicFileContext>();
        musicFiles = new ObservableCollection<MusicFile>()
        {
            musicFileContext.Create(@"C:\Users\Public\Music\Dancefloor\Culture Beat - Serenity.wav"),
            musicFileContext.Create(@"C:\Culture Beat - Serenity - Epilog.wma"),
        };
        selectionService = Container.GetExportedValue<SelectionService>();
        selectionService.Initialize(musicFiles);

        playlistManager = new PlaylistManager();
        playlistSettings = new PlaylistSettings();
        controller = Container.GetExportedValue<PlayerController>();
        controller.PlaylistSettings = playlistSettings;
        controller.PlaylistManager = playlistManager;
        controller.Initialize();

        shellService = Container.GetExportedValue<ShellService>();
        shellService.ShowPlaylistViewAction = () => { };
        shellService.ShowMusicPropertiesViewAction = () => { };
        view = (MockPlayerView)shellService.PlayerView!;
        viewModel = ViewHelper.GetViewModel<PlayerViewModel>(view)!;
    }

    protected override void OnCleanup()
    {
        controller.Shutdown();
        Assert.IsNull(playlistManager.CurrentItem);
        base.OnCleanup();
    }

    [TestMethod]
    public void PassMusicFileViaCommandLineParameter()
    {
        // Simulate that a music file was passed via command line parameter
        var environmentService = Container.GetExportedValue<MockEnvironmentService>();
        environmentService.MusicFilesToLoad = new[] { musicFiles.First().FileName! };

        // Another controller is responsible to add first the item into the playlist.
        playlistManager.InsertItems(0, musicFiles.Select(x => new PlaylistItem(x)));

        // Now auto-play the file.
        var playerService = Container.GetExportedValue<PlayerService>();
        playerService.IsPlayCommand = true;
        bool playPauseCommandCalled = false;
        playerService.PlayPauseCommand = new DelegateCommand(() => playPauseCommandCalled = true);

        controller.Run();
        Assert.AreEqual(musicFiles[0], playlistManager.CurrentItem!.MusicFile);
        Assert.IsTrue(playPauseCommandCalled);
    }

    [TestMethod]
    public void ResumePlaying()
    {
        playlistSettings.LastPlayedFileName = musicFiles.First().FileName;
        playlistSettings.LastPlayedFilePosition = TimeSpan.FromSeconds(432);

        // Another controller is responsible to add first the item into the playlist.
        playlistManager.InsertItems(0, musicFiles.Select(x => new PlaylistItem(x)));

        controller.Run();
        Assert.AreEqual(musicFiles[0], playlistManager.CurrentItem!.MusicFile);
        Assert.AreEqual(TimeSpan.FromSeconds(432), view.Position);
    }

    [TestMethod]
    public void PlayAllAndSelected()
    {
        controller.Run();
        Assert.IsTrue(viewModel.PlayerService.PlayAllCommand.CanExecute(null));
        Assert.IsFalse(viewModel.PlayerService.PlaySelectedCommand.CanExecute(null));

        // Select first music file
        AssertHelper.CanExecuteChangedEvent(viewModel.PlayerService.PlaySelectedCommand, () => selectionService.SelectedMusicFiles.Add(selectionService.MusicFiles[0]));
        Assert.IsTrue(viewModel.PlayerService.PlaySelectedCommand.CanExecute(null));

        // Insert a dummy playlist item which will be replaced by Play
        playlistManager.AddAndReplaceItems(new[] { new PlaylistItem(MockMusicFile.CreateEmpty("empty.mp3")) });
        Assert.AreEqual(1, playlistManager.Items.Count);

        // Play all
        var playerService = Container.GetExportedValue<PlayerService>();
        playerService.IsPlayCommand = true;
        bool playPauseCommandCalled = false;
        playerService.PlayPauseCommand = new DelegateCommand(() => playPauseCommandCalled = true);

        bool showPlaylistViewCalled = false;
        shellService.ShowPlaylistViewAction = () => showPlaylistViewCalled = true;

        viewModel.PlayerService.PlayAllCommand.Execute(null);
        Assert.IsTrue(playPauseCommandCalled);
        Assert.IsTrue(showPlaylistViewCalled);
        AssertHelper.SequenceEqual(musicFiles, playlistManager.Items.Select(x => x.MusicFile));
        Assert.AreEqual(musicFiles.First(), playlistManager.CurrentItem!.MusicFile);
    }

    [TestMethod]
    public void EnqueueAllAndSelected()
    {
        controller.Run();
        Assert.IsTrue(viewModel.PlayerService.EnqueueAllCommand.CanExecute(null));
        Assert.IsFalse(viewModel.PlayerService.EnqueueSelectedCommand.CanExecute(null));

        // Select first music file
        AssertHelper.CanExecuteChangedEvent(viewModel.PlayerService.PlaySelectedCommand, () => selectionService.SelectedMusicFiles.Add(selectionService.MusicFiles[0]));
        Assert.IsTrue(viewModel.PlayerService.EnqueueSelectedCommand.CanExecute(null));

        // Enqueue the selected music file
        bool showPlaylistViewCalled = false;
        shellService.ShowPlaylistViewAction = () => showPlaylistViewCalled = true;

        viewModel.PlayerService.EnqueueSelectedCommand.Execute(null);
        Assert.IsTrue(showPlaylistViewCalled);
        Assert.AreEqual(musicFiles.First(), playlistManager.Items.Single().MusicFile);
        Assert.IsNull(playlistManager.CurrentItem);

        // Enqueue all music files
        showPlaylistViewCalled = false;
        viewModel.PlayerService.EnqueueAllCommand.Execute(null);
        Assert.IsTrue(showPlaylistViewCalled);
        AssertHelper.SequenceEqual(new[] { musicFiles[0] }.Concat(musicFiles), playlistManager.Items.Select(x => x.MusicFile));
        Assert.IsNull(playlistManager.CurrentItem);
    }

    [TestMethod]
    public void PreviousAndNextTrackCommand()
    {
        controller.Run();
        Assert.IsFalse(viewModel.PreviousTrackCommand.CanExecute(null));
        Assert.IsFalse(viewModel.NextTrackCommand.CanExecute(null));
        Assert.IsNull(viewModel.PlayerService.PlayingMusicFile);

        AssertHelper.CanExecuteChangedEvent(viewModel.NextTrackCommand, () => viewModel.PlayerService.PlayAllCommand.Execute(null));
        Assert.IsFalse(viewModel.PreviousTrackCommand.CanExecute(null));
        Assert.IsTrue(viewModel.NextTrackCommand.CanExecute(null));
        Assert.AreEqual(musicFiles[0], viewModel.PlayerService.PlayingMusicFile);

        viewModel.NextTrackCommand.Execute(null);
        Assert.IsTrue(viewModel.PreviousTrackCommand.CanExecute(null));
        Assert.IsFalse(viewModel.NextTrackCommand.CanExecute(null));
        Assert.AreEqual(musicFiles[1], viewModel.PlayerService.PlayingMusicFile);

        viewModel.PreviousTrackCommand.Execute(null);
        Assert.IsFalse(viewModel.PreviousTrackCommand.CanExecute(null));
        Assert.IsTrue(viewModel.NextTrackCommand.CanExecute(null));
        Assert.AreEqual(musicFiles[0], viewModel.PlayerService.PlayingMusicFile);
    }

    [TestMethod]
    public void ShowInfoView()
    {
        controller.Run();
        bool showInfoViewCalled = false;
        MockInfoView.ShowDialogAction = view => showInfoViewCalled = true;

        viewModel.InfoCommand.Execute(null);
        Assert.IsTrue(showInfoViewCalled);

        MockInfoView.ShowDialogAction = null;
    }

    [TestMethod]
    public void ShowMusicProperties()
    {
        controller.Run();
        viewModel.PlayerService.PlayAllCommand.Execute(null);

        bool showMusicPropertiesViewActionCalled = false;
        shellService.ShowMusicPropertiesViewAction = () => { showMusicPropertiesViewActionCalled = true; };
        viewModel.ShowMusicPropertiesCommand.Execute(null);
        Assert.IsTrue(showMusicPropertiesViewActionCalled);
        var musicPropertiesViewModel = Container.GetExportedValue<MusicPropertiesViewModel>();
        Assert.AreEqual(playlistManager.CurrentItem!.MusicFile, musicPropertiesViewModel.MusicFile);
    }

    [TestMethod]
    public void ShowPlaylist()
    {
        controller.Run();
        viewModel.PlayerService.PlayAllCommand.Execute(null);
        shellService.ShowMusicPropertiesView();

        var playlistController = Container.GetExportedValue<PlaylistController>();
        playlistController.PlaylistManager = playlistManager;
        playlistController.Initialize();

        bool showPlaylistViewActionCalled = false;
        shellService.ShowPlaylistViewAction = () => { showPlaylistViewActionCalled = true; };
        viewModel.ShowPlaylistCommand.Execute(null);
        Assert.IsTrue(showPlaylistViewActionCalled);

        var playlistViewModel = Container.GetExportedValue<PlaylistViewModel>();
        Assert.AreEqual(playlistManager.CurrentItem, playlistViewModel.SelectedPlaylistItem);
    }
}
