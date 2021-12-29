using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Applications.Services;

[Export(typeof(IPlayerService)), Export]
public class PlayerService : Model, IPlayerService
{
    private ICommand playAllCommand = DelegateCommand.DisabledCommand;
    private ICommand playSelectedCommand = DelegateCommand.DisabledCommand;
    private ICommand enqueueAllCommand = DelegateCommand.DisabledCommand;
    private ICommand enqueueSelectedCommand = DelegateCommand.DisabledCommand;
    private ICommand previousCommand = DelegateCommand.DisabledCommand;
    private ICommand playPauseCommand = DelegateCommand.DisabledCommand;
    private ICommand nextCommand = DelegateCommand.DisabledCommand;
    private bool isPlayCommand;
    private MusicFile? playingMusicFile;

    public ICommand PlayAllCommand
    {
        get => playAllCommand;
        set => SetProperty(ref playAllCommand, value);
    }

    public ICommand PlaySelectedCommand
    {
        get => playSelectedCommand;
        set => SetProperty(ref playSelectedCommand, value);
    }

    public ICommand EnqueueAllCommand
    {
        get => enqueueAllCommand;
        set => SetProperty(ref enqueueAllCommand, value);
    }

    public ICommand EnqueueSelectedCommand
    {
        get => enqueueSelectedCommand;
        set => SetProperty(ref enqueueSelectedCommand, value);
    }

    public ICommand PreviousCommand
    {
        get => previousCommand;
        set => SetProperty(ref previousCommand, value);
    }

    public ICommand PlayPauseCommand
    {
        get => playPauseCommand;
        set => SetProperty(ref playPauseCommand, value);
    }

    public ICommand NextCommand
    {
        get => nextCommand;
        set => SetProperty(ref nextCommand, value);
    }

    public bool IsPlayCommand
    {
        get => isPlayCommand;
        set => SetProperty(ref isPlayCommand, value);
    }

    public MusicFile? PlayingMusicFile
    {
        get => playingMusicFile;
        set => SetProperty(ref playingMusicFile, value);
    }
}
