using System;
using System.Waf.Foundation;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Domain.Transcoding
{
    public class TranscodeItem : Model
    {
        private readonly MusicFile source;
        private readonly string destinationFileName;
        private TranscodeStatus transcodeStatus;
        private double progress;
        private Exception error;


        public TranscodeItem(MusicFile source, string destinationFileName)
        {
            this.source = source;
            this.destinationFileName = destinationFileName;
            UpdateStatus();
        }


        public MusicFile Source { get { return source; } }

        public string DestinationFileName { get { return destinationFileName; } }

        public TranscodeStatus TranscodeStatus
        {
            get { return transcodeStatus; }
            private set { SetProperty(ref transcodeStatus, value); }
        }

        public double Progress
        {
            get { return progress; }
            set 
            {
                if (SetProperty(ref progress, value))
                {
                    UpdateStatus();
                }
            }
        }

        public Exception Error
        {
            get { return error; }
            set 
            { 
                if (SetProperty(ref error, value))
                {
                    UpdateStatus();
                }
            }
        }


        private void UpdateStatus()
        {
            if (Error != null) 
            { 
                TranscodeStatus = Transcoding.TranscodeStatus.Error; 
            }
            else if (Progress == 0) 
            { 
                TranscodeStatus = Transcoding.TranscodeStatus.Pending; 
            }
            else if (Progress < 1) 
            { 
                TranscodeStatus = Transcoding.TranscodeStatus.InProgress; 
            }
            else 
            { 
                TranscodeStatus = Transcoding.TranscodeStatus.Completed; 
            }
        }
    }
}
