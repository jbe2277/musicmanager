using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.MusicManager.Applications.Views;
using Test.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Domain.Playlists;

namespace Test.MusicManager.Applications.ViewModels;

[TestClass]
public class PlaylistViewModelTest : ApplicationsTest
{
    [TestMethod]
    public void SearchTextTest()
    {
        var viewModel = Get<PlaylistViewModel>();
        var view = Get<MockPlaylistView>();
        PlaylistItem? scrollIntoViewItem = null;
        view.ScrollIntoViewStub = x => scrollIntoViewItem = x;
        bool focusSearchBoxCalled = false;
        view.FocusSearchBoxStub = () => focusSearchBoxCalled = true;

        var musicFiles = new[]
        {
            MockMusicFile.CreateEmpty(@"C:\Users\Public\Music\Dancefloor\Culture Beat - Serenity.wav"),
            MockMusicFile.CreateEmpty(@"C:\Future Breeze - Why don't you dance with me.mp3"),
            MockMusicFile.CreateEmpty(@"C:\Culture Beat - Serenity - Epilog.wma"),
        };
        var playlistManager = new PlaylistManager();
        playlistManager.AddAndReplaceItems(musicFiles.Select(x => new PlaylistItem(x)));
        viewModel.PlaylistManager = playlistManager;
            
        viewModel.SearchText = "Bill";
        Assert.IsNull(viewModel.SelectedPlaylistItem);
        Assert.IsNull(scrollIntoViewItem);
        Assert.IsTrue(focusSearchBoxCalled);

        viewModel.SearchText = "Cul";
        Assert.AreEqual(playlistManager.Items[0], viewModel.SelectedPlaylistItem);
        Assert.AreEqual(playlistManager.Items[0], scrollIntoViewItem);

        // Selection stays on the same item
        viewModel.SearchText = "Cultur";
        Assert.AreEqual(playlistManager.Items[0], viewModel.SelectedPlaylistItem);
        Assert.AreEqual(playlistManager.Items[0], scrollIntoViewItem);

        viewModel.SearchText = "Epi";
        Assert.AreEqual(playlistManager.Items[2], viewModel.SelectedPlaylistItem);
        Assert.AreEqual(playlistManager.Items[2], scrollIntoViewItem);

        // Selection stays on the same item
        scrollIntoViewItem = null;
        viewModel.SearchText = "";
        Assert.AreEqual(playlistManager.Items[2], viewModel.SelectedPlaylistItem);
        Assert.IsNull(scrollIntoViewItem);

        viewModel.SelectedPlaylistItem = playlistManager.Items[1];

        // Search starts from selection
        viewModel.SearchText = "Cul";
        Assert.AreEqual(playlistManager.Items[2], viewModel.SelectedPlaylistItem);
        Assert.AreEqual(playlistManager.Items[2], scrollIntoViewItem);
    }

    [TestMethod]
    public void SearchNextAndPrevious()
    {
        var viewModel = Get<PlaylistViewModel>();
        var view = Get<MockPlaylistView>();
            
        var musicFiles = new[]
        {
            MockMusicFile.CreateEmpty(@"C:\Culture Beat"),
            MockMusicFile.CreateEmpty(@"C:\Culture Beat"),
            MockMusicFile.CreateEmpty(@"C:\Culture Beat"),
            MockMusicFile.CreateEmpty(@"C:\Foo"),
        };
        var playlistManager = new PlaylistManager();
        playlistManager.AddAndReplaceItems(musicFiles.Select(x => new PlaylistItem(x)));
        viewModel.PlaylistManager = playlistManager;

        viewModel.SearchText = "Culture";
        Assert.AreEqual(playlistManager.Items[0], viewModel.SelectedPlaylistItem);

        viewModel.SearchNextCommand.Execute(null);
        Assert.AreEqual(playlistManager.Items[1], viewModel.SelectedPlaylistItem);

        viewModel.SearchNextCommand.Execute(null);
        Assert.AreEqual(playlistManager.Items[2], viewModel.SelectedPlaylistItem);
            
        viewModel.SearchNextCommand.Execute(null);
        Assert.AreEqual(playlistManager.Items[0], viewModel.SelectedPlaylistItem);

        viewModel.SearchPreviousCommand.Execute(null);
        Assert.AreEqual(playlistManager.Items[2], viewModel.SelectedPlaylistItem);

        viewModel.SearchPreviousCommand.Execute(null);
        Assert.AreEqual(playlistManager.Items[1], viewModel.SelectedPlaylistItem);

        viewModel.SearchPreviousCommand.Execute(null);
        Assert.AreEqual(playlistManager.Items[0], viewModel.SelectedPlaylistItem);
    }

    [TestMethod]
    public void SearchTextIsContainedTest()
    {
        var viewModel = Get<PlaylistViewModel>();
        var view = Get<MockPlaylistView>();
        PlaylistItem? scrollIntoViewItem = null;
        view.ScrollIntoViewStub = x => scrollIntoViewItem = x;

        var musicFiles = new[]
        {
            CreateMockMusicFile(@"C:\Music\Foo.mp3", "1 title", "1 artist"),
            CreateMockMusicFile(@"C:\Music\Baz.wma", "2 TiTle", "2 ArTisT"),
            CreateMockMusicFile(@"C:\Music\Abc.mp3", "3 TITLE", "3a artist", "3B ARTIST"),
            new MusicFile(x => Task.FromResult((MusicMetadata?)null), "do not load"),
        };
        var playlistManager = new PlaylistManager();
        playlistManager.AddAndReplaceItems(musicFiles.Select(x => new PlaylistItem(x)));
        viewModel.PlaylistManager = playlistManager;

        viewModel.SearchText = "1 TIT";
        Assert.AreEqual(playlistManager.Items[0], viewModel.SelectedPlaylistItem);
        Assert.AreEqual(playlistManager.Items[0], scrollIntoViewItem);

        viewModel.SearchText = "1 ART";
        Assert.AreEqual(playlistManager.Items[0], viewModel.SelectedPlaylistItem);
        Assert.AreEqual(playlistManager.Items[0], scrollIntoViewItem);

        viewModel.SearchText = "3B art";
        Assert.AreEqual(playlistManager.Items[2], viewModel.SelectedPlaylistItem);
        Assert.AreEqual(playlistManager.Items[2], scrollIntoViewItem);

        // File name is not considered as Title & Artist are set and shown.
        scrollIntoViewItem = null;
        viewModel.SearchText = "baz";
        Assert.AreEqual(playlistManager.Items[2], viewModel.SelectedPlaylistItem);
        Assert.IsNull(scrollIntoViewItem);

        // Ensure that the search feature does not load the metadata of all Playlist items.
        Assert.IsFalse(musicFiles[^1].IsMetadataLoaded);
    }

    private static MusicFile CreateMockMusicFile(string fileName, string title, params string[] artists)
    {
        var metadata = new MusicMetadata(TimeSpan.FromSeconds(3), 128000) { Title = title, Artists = artists };
        return new MockMusicFile(metadata, fileName);
    }
}
