using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.UnitTesting.Mocks;
using Test.MusicManager.Applications.Views;
using Test.MusicManager.Domain.MusicFiles;
using Waf.MusicManager.Applications.Controllers;
using Waf.MusicManager.Applications.Properties;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Domain.Playlists;

namespace Test.MusicManager.Presentation.Controllers;

[TestClass]
public class ModuleControllerTest : PresentationTest
{
    private ModuleController controller = null!;
    private ShellService shellService = null!;
        
    protected override void OnInitialize()
    {
        base.OnInitialize();
        controller = Get<ModuleController>();
        shellService = Get<ShellService>();
        controller.Initialize();
        controller.Run();
    }

    protected override void OnCleanup()
    {
        controller.Shutdown();
        base.OnCleanup();
    }
        
    [TestMethod]
    public void LaodAndSaveSettings()
    {
        var appSettings = new AppSettings();
        var playlistSettings = new PlaylistSettings();
        var settingsService = Get<MockSettingsService>();
        settingsService.GetStub = type =>
        {
            if (type == typeof(AppSettings)) return appSettings;
            if (type == typeof(PlaylistSettings)) return playlistSettings;
            throw new NotSupportedException(type.Name);
        };

        controller.Initialize();
        controller.Run();

        var playerController = Get<PlayerController>();
        shellService.Settings.Height = 42;
        playerController.PlaylistManager.CurrentItem = new PlaylistItem(MockMusicFile.CreateEmpty("Test"));

        controller.Shutdown();

        shellService.Settings = null!;
        playerController.PlaylistSettings = null!;

        controller.Initialize();
        controller.Run();

        Assert.AreEqual(42, shellService.Settings.Height);
        Assert.AreEqual("Test", playerController.PlaylistSettings.LastPlayedFileName);
    }
        
    [TestMethod]
    public void ShowDetailViewsTest()
    {
        var view = Get<MockShellView>();
        Assert.IsTrue(view.IsVisible);
            
        var viewModel = Get<ShellViewModel>();
        Assert.IsFalse(viewModel.IsMusicPropertiesViewVisible);
        Assert.IsTrue(viewModel.IsPlaylistViewVisible);
        Assert.IsFalse(viewModel.IsTranscodingListViewVisible);

            
        shellService.ShowMusicPropertiesView();
        Assert.IsTrue(viewModel.IsMusicPropertiesViewVisible);
        Assert.IsFalse(viewModel.IsPlaylistViewVisible);
        Assert.IsFalse(viewModel.IsTranscodingListViewVisible);

        shellService.ShowTranscodingListView();
        Assert.IsFalse(viewModel.IsMusicPropertiesViewVisible);
        Assert.IsFalse(viewModel.IsPlaylistViewVisible);
        Assert.IsTrue(viewModel.IsTranscodingListViewVisible);

        shellService.ShowPlaylistView();
        Assert.IsFalse(viewModel.IsMusicPropertiesViewVisible);
        Assert.IsTrue(viewModel.IsPlaylistViewVisible);
        Assert.IsFalse(viewModel.IsTranscodingListViewVisible);
    }
}
