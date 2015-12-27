using System.Waf.Foundation;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Domain.Playlists
{
    public class PlaylistItem : Model
    {
        private readonly MusicFile musicFile;
        

        public PlaylistItem(MusicFile musicFile)
        {
            this.musicFile = musicFile;
        }


        public MusicFile MusicFile { get { return musicFile; } }
    }
}
