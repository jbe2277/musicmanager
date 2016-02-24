using System;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.Playlists;

namespace Waf.MusicManager.Applications.ViewModels
{
    [Export]
    public class PlayerViewModel : ViewModel<IPlayerView>
    {
        private PlaylistManager playlistManager;
        private ICommand previousTrackCommand;
        private ICommand nextTrackCommand;
        private ICommand infoCommand;
        private ICommand showMusicPropertiesCommand;
        private double volume;

        
        [ImportingConstructor]
        public PlayerViewModel(IPlayerView view, IShellService shellService, IPlayerService playerService) : base(view)
        {
            ShellService = shellService;
            PlayerService = playerService;
        }


        public IShellService ShellService { get; }

        public IPlayerService PlayerService { get; }

        public PlaylistManager PlaylistManager
        {
            get { return playlistManager; }
            set { SetProperty(ref playlistManager, value); } 
        }

        public ICommand PreviousTrackCommand
        {
            get { return previousTrackCommand; }
            set { SetProperty(ref previousTrackCommand, value); }
        }

        public ICommand NextTrackCommand
        {
            get { return nextTrackCommand; }
            set { SetProperty(ref nextTrackCommand, value); }
        }

        public ICommand InfoCommand
        {
            get { return infoCommand; }
            set { SetProperty(ref infoCommand, value); }
        }

        public ICommand ShowMusicPropertiesCommand
        {
            get { return showMusicPropertiesCommand; }
            set { SetProperty(ref showMusicPropertiesCommand, value); }
        }

        public double Volume
        {
            get { return volume; }
            set { SetProperty(ref volume, value); }
        }


        public TimeSpan GetPosition() { return ViewCore.GetPosition(); }

        public void SetPosition(TimeSpan position) { ViewCore.SetPosition(position); }
    }
}
