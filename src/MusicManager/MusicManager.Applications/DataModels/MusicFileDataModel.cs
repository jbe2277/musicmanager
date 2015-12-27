using System.ComponentModel;
using System.Globalization;
using System.Waf.Foundation;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Applications.DataModels
{
    public class MusicFileDataModel : Model
    {
        private readonly MusicFile musicFile;


        public MusicFileDataModel(MusicFile musicFile)
        {
            this.musicFile = musicFile;
            if (musicFile.IsMetadataLoaded)
            {
                MetadataLoaded();
            }
            else
            {
                PropertyChangedEventManager.AddHandler(musicFile, MusicFilePropertyChanged, "");
            }
        }


        public MusicFile MusicFile { get { return musicFile; } }

        public string ArtistsString 
        { 
            get 
            { 
                return string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator + " ", 
                    MusicFile.IsMetadataLoaded ? MusicFile.Metadata.Artists : new string[0]); 
            }
        }


        private void MetadataLoaded()
        {
            PropertyChangedEventManager.AddHandler(musicFile.Metadata, MetadataPropertyChanged, "");
        }

        private void MusicFilePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsMetadataLoaded")
            {
                PropertyChangedEventManager.RemoveHandler(musicFile, MusicFilePropertyChanged, "");
                MetadataLoaded();
                RaisePropertyChanged("ArtistsString");
            }
        }

        private void MetadataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Artists")
            {
                RaisePropertyChanged("ArtistsString");
            }
        }
    }
}
