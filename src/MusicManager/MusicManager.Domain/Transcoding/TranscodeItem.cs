using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Domain.Transcoding
{
    public class TranscodeItem : Model
    {
        private TranscodeStatus transcodeStatus;
        private double progress;
        private Exception? error;

        public TranscodeItem(MusicFile source, string destinationFileName)
        {
            Source = source;
            DestinationFileName = destinationFileName;
            UpdateStatus();
        }

        public MusicFile Source { get; }

        public string DestinationFileName { get; }

        public TranscodeStatus TranscodeStatus
        {
            get => transcodeStatus;
            private set => SetProperty(ref transcodeStatus, value);
        }

        public double Progress
        {
            get => progress;
            set
            {
                if (!SetProperty(ref progress, value)) return;
                UpdateStatus();
            }
        }

        public Exception? Error
        {
            get => error;
            set
            {
                if (!SetProperty(ref error, value)) return;
                UpdateStatus();
            }
        }

        private void UpdateStatus()
        {
            TranscodeStatus = this switch
            {
                { Error: not null } => TranscodeStatus.Error,
                { Progress: 0 } => TranscodeStatus.Pending,
                { Progress: < 1} => TranscodeStatus.InProgress,
                _ => TranscodeStatus.Completed
            };
        }
    }
}
