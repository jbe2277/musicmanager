using System.Waf.Applications;
using System.Windows.Input;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.Playlists;

namespace Waf.MusicManager.Applications.ViewModels;

public class PlayerViewModel : ViewModel<IPlayerView>
{
    private PlaylistManager playlistManager = null!;
    private ICommand previousTrackCommand = DelegateCommand.DisabledCommand;
    private ICommand nextTrackCommand = DelegateCommand.DisabledCommand;
    private ICommand infoCommand = DelegateCommand.DisabledCommand;
    private ICommand showMusicPropertiesCommand = DelegateCommand.DisabledCommand;
    private ICommand showPlaylistCommand = DelegateCommand.DisabledCommand;
    private double volume;

    public PlayerViewModel(IPlayerView view, IShellService shellService, IPlayerService playerService) : base(view)
    {
        ShellService = shellService;
        PlayerService = playerService;
    }

    public IShellService ShellService { get; }

    public IPlayerService PlayerService { get; }

    public PlaylistManager PlaylistManager
    {
        get => playlistManager;
        set => SetProperty(ref playlistManager, value);
    }

    public ICommand PreviousTrackCommand
    {
        get => previousTrackCommand;
        set => SetProperty(ref previousTrackCommand, value);
    }

    public ICommand NextTrackCommand
    {
        get => nextTrackCommand;
        set => SetProperty(ref nextTrackCommand, value);
    }

    public ICommand InfoCommand
    {
        get => infoCommand;
        set => SetProperty(ref infoCommand, value);
    }

    public ICommand ShowMusicPropertiesCommand
    {
        get => showMusicPropertiesCommand;
        set => SetProperty(ref showMusicPropertiesCommand, value);
    }

    public ICommand ShowPlaylistCommand
    {
        get => showPlaylistCommand;
        set => SetProperty(ref showPlaylistCommand, value);
    }

    public double Volume
    {
        get => volume;
        set => SetProperty(ref volume, value);
    }

    public TimeSpan GetPosition() => ViewCore.GetPosition();

    public void SetPosition(TimeSpan position) => ViewCore.SetPosition(position);
}
