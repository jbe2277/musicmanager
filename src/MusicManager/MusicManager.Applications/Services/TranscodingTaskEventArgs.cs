namespace Waf.MusicManager.Applications.Services;

public class TranscodingTaskEventArgs(string fileName, Task transcodingTask) : EventArgs
{
    public string FileName { get; } = fileName;

    public Task TranscodingTask { get; } = transcodingTask;
}
