using System.Waf.Applications;
using System.Windows.Input;
using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Applications.Views;
using Waf.MusicManager.Domain.Playlists;

namespace Waf.MusicManager.Applications.ViewModels;

public class PlayerViewModel(IPlayerView view, IShellService shellService, IPlayerService playerService) : ViewModel<IPlayerView>(view)
{
    public IShellService ShellService { get; } = shellService;

    public IPlayerService PlayerService { get; } = playerService;

    public PlaylistManager PlaylistManager { get; set => SetProperty(ref field, value); } = null!;

    public ICommand PreviousTrackCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand NextTrackCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand InfoCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand ShowMusicPropertiesCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand ShowPlaylistCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public double Volume { get; set => SetProperty(ref field, value); }

    public TimeSpan GetPosition() => ViewCore.GetPosition();

    public void SetPosition(TimeSpan position) => ViewCore.SetPosition(position);
}
