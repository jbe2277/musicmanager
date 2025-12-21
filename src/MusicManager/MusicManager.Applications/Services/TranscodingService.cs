using System.Waf.Applications;
using System.Windows.Input;

namespace Waf.MusicManager.Applications.Services;

internal class TranscodingService : Model, ITranscodingService
{
    public ICommand ConvertToMp3AllCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand ConvertToMp3SelectedCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand CancelAllCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public ICommand CancelSelectedCommand { get; set => SetProperty(ref field, value); } = DelegateCommand.DisabledCommand;

    public event EventHandler<TranscodingTaskEventArgs>? TranscodingTaskCreated;

    public void RaiseTranscodingTaskCreated(string fileName, Task transcodingTask) => OnTranscodingTaskCreated(new(fileName, transcodingTask));

    protected virtual void OnTranscodingTaskCreated(TranscodingTaskEventArgs e) => TranscodingTaskCreated?.Invoke(this, e);
}
