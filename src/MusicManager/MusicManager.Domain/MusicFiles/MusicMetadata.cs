namespace Waf.MusicManager.Domain.MusicFiles;

public class MusicMetadata : Entity
{
    private MusicMetadata(TimeSpan duration, long bitrate, bool isSupported)
    {
        IsSupported = isSupported;
        Duration = duration;
        Bitrate = bitrate;
    }

    public MusicMetadata(TimeSpan duration, long bitrate) : this(duration, bitrate, true)
    {
    }

    public static MusicMetadata CreateUnsupported(TimeSpan duration, long bitrate) => new(duration, bitrate, false);

    public object Parent { get; set; } = null!;

    public bool IsSupported { get; }

    public IReadOnlyList<string> Artists { get; set => SetPropertyAndTrackChanges(ref field, value); } = [];

    public string Title { get; set => SetPropertyAndTrackChanges(ref field, value); } = "";

    public TimeSpan Duration { get; }

    public uint Rating { get; set => SetPropertyAndTrackChanges(ref field, value); }

    public string Album { get; set => SetPropertyAndTrackChanges(ref field, value); } = "";

    public uint TrackNumber { get; set => SetPropertyAndTrackChanges(ref field, value); }

    public uint Year { get; set => SetPropertyAndTrackChanges(ref field, value); }

    public IReadOnlyList<string> Genre { get; set => SetPropertyAndTrackChanges(ref field, value); } = [];

    public long Bitrate { get; }

    public string AlbumArtist { get; set => SetPropertyAndTrackChanges(ref field, value); } = "";

    public string Publisher { get; set => SetPropertyAndTrackChanges(ref field, value); } = "";

    public string Subtitle { get; set => SetPropertyAndTrackChanges(ref field, value); } = "";

    public IReadOnlyList<string> Composers { get; set => SetPropertyAndTrackChanges(ref field, value); } = [];

    public IReadOnlyList<string> Conductors { get; set => SetPropertyAndTrackChanges(ref field, value); } = [];

    public void ApplyValuesFrom(MusicMetadata sourceMetadata)
    {
        Artists = sourceMetadata.Artists;
        Title = sourceMetadata.Title;
        Rating = sourceMetadata.Rating;
        Album = sourceMetadata.Album;
        TrackNumber = sourceMetadata.TrackNumber;
        Year = sourceMetadata.Year;
        Genre = sourceMetadata.Genre;
        AlbumArtist = sourceMetadata.AlbumArtist;
        Publisher = sourceMetadata.Publisher;
        Subtitle = sourceMetadata.Subtitle;
        Composers = sourceMetadata.Composers;
        Conductors = sourceMetadata.Conductors;
    }
}
