using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Waf.Applications;
using System.Windows.Threading;
using Waf.MusicManager.Applications.Properties;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.ViewModels;
using Waf.MusicManager.Domain;
using Waf.MusicManager.Domain.Playlists;

namespace Waf.MusicManager.Applications.Controllers
{
    [Export(typeof(IModuleController)), Export]
    internal class ModuleController : IModuleController
    {
        private const string appSettingsFileName = "Settings.xml";
        private const string playlistSettingsFileName = "Playlist.xml";
        
        private readonly Lazy<ShellService> shellService;
        private readonly IEnvironmentService environmentService;
        private readonly ISettingsProvider settingsProvider;
        private readonly Lazy<ManagerController> managerController;
        private readonly Lazy<MusicPropertiesController> musicPropertiesController;
        private readonly Lazy<PlayerController> playerController;
        private readonly Lazy<PlaylistController> playlistController;
        private readonly Lazy<TranscodingController> transcodingController;
        private readonly Lazy<ShellViewModel> shellViewModel;
        private readonly PlaylistManager playlistManager;
        private AppSettings appSettings;
        private PlaylistSettings playlistSettings;
        
        
        [ImportingConstructor]
        public ModuleController(Lazy<ShellService> shellService, IEnvironmentService environmentService, ISettingsProvider settingsProvider, Lazy<ManagerController> managerController, 
            Lazy<MusicPropertiesController> musicPropertiesController, Lazy<PlayerController> playerController, Lazy<PlaylistController> playlistController, 
            Lazy<TranscodingController> transcodingController, Lazy<ShellViewModel> shellViewModel)
        {
            this.shellService = shellService;
            this.environmentService = environmentService;
            this.settingsProvider = settingsProvider;
            this.managerController = managerController;
            this.musicPropertiesController = musicPropertiesController;
            this.playerController = playerController;
            this.playlistController = playlistController;
            this.transcodingController = transcodingController;
            this.shellViewModel = shellViewModel;
            this.playlistManager = new PlaylistManager();
        }


        private ShellService ShellService { get { return shellService.Value; } }

        private ManagerController ManagerController { get { return managerController.Value; } }

        private MusicPropertiesController MusicPropertiesController { get { return musicPropertiesController.Value; } }

        private PlayerController PlayerController { get { return playerController.Value; } }

        private PlaylistController PlaylistController { get { return playlistController.Value; } }

        private TranscodingController TranscodingController { get { return transcodingController.Value; } }

        private ShellViewModel ShellViewModel { get { return shellViewModel.Value; } }


        public void Initialize()
        {
            appSettings = LoadSettings<AppSettings>(appSettingsFileName);
            playlistSettings = LoadSettings<PlaylistSettings>(playlistSettingsFileName);

            ShellService.Settings = appSettings;
            ShellService.ShowErrorAction = ShellViewModel.ShowError;
            ShellService.ShowMusicPropertiesViewAction = ShowMusicPropertiesView;
            ShellService.ShowPlaylistViewAction = ShowPlaylistView;
            ShellService.ShowTranscodingListViewAction = ShowTranscodingListView;

            ManagerController.Initialize();
            MusicPropertiesController.PlaylistManager = playlistManager;
            MusicPropertiesController.Initialize();
            PlayerController.PlaylistManager = playlistManager;
            PlayerController.PlaylistSettings = playlistSettings;
            PlayerController.Initialize();
            PlaylistController.PlaylistSettings = playlistSettings;
            PlaylistController.PlaylistManager = playlistManager;
            PlaylistController.Initialize();
            TranscodingController.Initialize();
        }

        public async void Run()
        {
            ShellViewModel.IsPlaylistViewVisible = true;
            ShellViewModel.Show();

            // Let the UI to initialize first before loading the playlist.
            await Dispatcher.CurrentDispatcher.InvokeAsync(PlaylistController.Run, DispatcherPriority.ApplicationIdle);
            
            // The UI must be initialized completely before we continue to load the last opened music file.
            await Dispatcher.CurrentDispatcher.InvokeAsync(PlayerController.Run, DispatcherPriority.ApplicationIdle);
        }

        public void Shutdown()
        {
            // Call this method before the player is stopped. It ensures that the App stays alive until the playing file is saved as well.
            MusicPropertiesController.Shutdown();

            TranscodingController.Shutdown();
            PlaylistController.Shutdown();
            PlayerController.Shutdown();
            ManagerController.Shutdown();
            
            SaveSettings(appSettingsFileName, appSettings);
            SaveSettings(playlistSettingsFileName, playlistSettings);
        }

        private T LoadSettings<T>(string fileName) where T : class, new()
        {
            try
            {
                return settingsProvider.LoadSettings<T>(Path.Combine(environmentService.AppSettingsPath, fileName));
            }
            catch (Exception ex)
            {
                Logger.Error("Could not read the settings file: {0}", ex);
                return new T();
            }
        }

        private void SaveSettings(string fileName, object settings)
        {
            try
            {
                settingsProvider.SaveSettings(Path.Combine(environmentService.AppSettingsPath, fileName), settings);
            }
            catch (Exception ex)
            {
                Logger.Error("Could not save the settings file: {0}", ex);
            }
        }

        private void ShowMusicPropertiesView()
        {
            ShellViewModel.IsMusicPropertiesViewVisible = true;
        }

        private void ShowPlaylistView()
        {
            ShellViewModel.IsPlaylistViewVisible = true;
        }

        private void ShowTranscodingListView()
        {
            ShellViewModel.IsTranscodingListViewVisible = true;
        }
    }
}
