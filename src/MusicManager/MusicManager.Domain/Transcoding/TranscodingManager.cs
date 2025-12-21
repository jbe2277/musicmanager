namespace Waf.MusicManager.Domain.Transcoding;

public class TranscodingManager
{
    private readonly ObservableList<TranscodeItem> transcodeItems = [];

    public TranscodingManager()
    {
        TranscodeItems = new ReadOnlyObservableList<TranscodeItem>(transcodeItems);
    }

    public IReadOnlyObservableList<TranscodeItem> TranscodeItems { get; }

    public void AddTranscodeItem(TranscodeItem item) => transcodeItems.Add(item);

    public void RemoveTranscodeItem(TranscodeItem item) => transcodeItems.Remove(item);
}
