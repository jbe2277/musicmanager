using System.Collections.ObjectModel;
using System.Waf.Foundation;

namespace Waf.MusicManager.Domain.Transcoding
{
    public class TranscodingManager
    {
        private readonly ObservableCollection<TranscodeItem> transcodeItems;
        private readonly ReadOnlyObservableList<TranscodeItem> readOnlyTranscodeItems;


        public TranscodingManager()
        {
            this.transcodeItems = new ObservableCollection<TranscodeItem>();
            this.readOnlyTranscodeItems = new ReadOnlyObservableList<TranscodeItem>(transcodeItems);
        }


        public IReadOnlyObservableList<TranscodeItem> TranscodeItems { get { return readOnlyTranscodeItems; } }


        public void AddTranscodeItem(TranscodeItem item)
        {
            transcodeItems.Add(item);
        }

        public void RemoveTranscodeItem(TranscodeItem item)
        {
            transcodeItems.Remove(item);
        }
    }
}
