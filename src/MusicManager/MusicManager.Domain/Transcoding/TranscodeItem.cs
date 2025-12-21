using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Domain.Transcoding;

public class TranscodeItem(MusicFile source, string destinationFileName) : Model
{
    public MusicFile Source { get; } = source;

    public string DestinationFileName { get; } = destinationFileName;

    public TranscodeStatus TranscodeStatus => this switch
    {
        { Error: not null } => TranscodeStatus.Error,
        { Progress: 0 } => TranscodeStatus.Pending,
        { Progress: < 1 } => TranscodeStatus.InProgress,
        _ => TranscodeStatus.Completed
    };

    public double Progress { get; set => SetProperty(ref field, value); }

    public Exception? Error { get; set => SetProperty(ref field, value); }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName is nameof(Progress) or nameof(Error)) RaisePropertyChanged(nameof(TranscodeStatus));
    }
}
