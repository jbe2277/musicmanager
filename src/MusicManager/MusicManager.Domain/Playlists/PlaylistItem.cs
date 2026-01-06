using System.Waf.Foundation;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Domain.Playlists;

public class PlaylistItem(MusicFile musicFile) : Model
{
    public MusicFile MusicFile { get; } = musicFile;
}
