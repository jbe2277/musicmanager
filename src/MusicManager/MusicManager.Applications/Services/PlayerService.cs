using System.Waf.Applications;
using System.Windows.Input;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Applications.Services;

public class PlayerService : Model, IPlayerService
{
    public ICommand PlayAllCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand PlaySelectedCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand EnqueueAllCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand EnqueueSelectedCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand PreviousCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand PlayPauseCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand NextCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public bool IsPlayCommand { get; set => SetProperty(ref field, value); } = true;

    public MusicFile? PlayingMusicFile { get; set => SetProperty(ref field, value); }
}
