using System.Waf.Applications;
using System.Windows.Input;

namespace Waf.MusicManager.Applications.Services;

internal class TranscodingService : Model, ITranscodingService
{
    private ICommand convertToMp3AllCommand = DelegateCommand.DisabledCommand;
    private ICommand convertToMp3SelectedCommand = DelegateCommand.DisabledCommand;
    private ICommand cancelAllCommand = DelegateCommand.DisabledCommand;
    private ICommand cancelSelectedCommand = DelegateCommand.DisabledCommand;

    public ICommand ConvertToMp3AllCommand
    {
        get => convertToMp3AllCommand;
        set => SetProperty(ref convertToMp3AllCommand, value);
    }

    public ICommand ConvertToMp3SelectedCommand
    {
        get => convertToMp3SelectedCommand;
        set => SetProperty(ref convertToMp3SelectedCommand, value);
    }

    public ICommand CancelAllCommand
    {
        get => cancelAllCommand;
        set => SetProperty(ref cancelAllCommand, value);
    }

    public ICommand CancelSelectedCommand
    {
        get => cancelSelectedCommand;
        set => SetProperty(ref cancelSelectedCommand, value);
    }

    public event EventHandler<TranscodingTaskEventArgs>? TranscodingTaskCreated;

    public void RaiseTranscodingTaskCreated(string fileName, Task transcodingTask) => OnTranscodingTaskCreated(new(fileName, transcodingTask));

    protected virtual void OnTranscodingTaskCreated(TranscodingTaskEventArgs e) => TranscodingTaskCreated?.Invoke(this, e);
}
