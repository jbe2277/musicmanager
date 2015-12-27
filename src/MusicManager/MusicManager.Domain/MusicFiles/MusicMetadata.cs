using System;
using System.Collections.Generic;

namespace Waf.MusicManager.Domain.MusicFiles
{
    public class MusicMetadata : Entity
    {
        private readonly bool isSupported;
        private readonly TimeSpan duration;
        private readonly long bitrate;
        private IReadOnlyList<string> artists = new string[0];
        private string title = "";
        private uint rating;
        private string album = "";
        private uint trackNumber;
        private uint year;
        private IReadOnlyList<string> genre = new string[0];
        private string albumArtist = "";
        private string publisher = "";
        private string subtitle = "";
        private IReadOnlyList<string> composers = new string[0];
        private IReadOnlyList<string> conductors = new string[0];

        
        private MusicMetadata(TimeSpan duration, long bitrate, bool isSupported)
        {
            this.isSupported = isSupported;
            this.duration = duration;
            this.bitrate = bitrate;
        }

        public MusicMetadata(TimeSpan duration, long bitrate) : this(duration, bitrate, true)
        {
        }

        public static MusicMetadata CreateUnsupported(TimeSpan duration, long bitrate)
        {
            return new MusicMetadata(duration, bitrate, false);
        }
        

        public bool IsSupported { get { return isSupported; } }

        public IReadOnlyList<string> Artists
        {
            get { return artists; }
            set { SetPropertyAndTrackChanges(ref artists, value); }
        }

        public string Title
        {
            get { return title; }
            set { SetPropertyAndTrackChanges(ref title, value); }
        }

        public TimeSpan Duration { get { return duration; } }

        public uint Rating
        {
            get { return rating; }
            set { SetPropertyAndTrackChanges(ref rating, value); }
        }

        public string Album
        {
            get { return album; }
            set { SetPropertyAndTrackChanges(ref album, value); }
        }

        public uint TrackNumber
        {
            get { return trackNumber; }
            set { SetPropertyAndTrackChanges(ref trackNumber, value); }
        }

        public uint Year
        {
            get { return year; }
            set { SetPropertyAndTrackChanges(ref year, value); }
        }

        public IReadOnlyList<string> Genre
        {
            get { return genre; }
            set { SetPropertyAndTrackChanges(ref genre, value); }
        }

        public long Bitrate { get { return bitrate; } }

        public string AlbumArtist
        {
            get { return albumArtist; }
            set { SetPropertyAndTrackChanges(ref albumArtist, value); }
        }

        public string Publisher
        {
            get { return publisher; }
            set { SetPropertyAndTrackChanges(ref publisher, value); }
        }

        public string Subtitle
        {
            get { return subtitle; }
            set { SetPropertyAndTrackChanges(ref subtitle, value); }
        }

        public IReadOnlyList<string> Composers
        {
            get { return composers; }
            set { SetPropertyAndTrackChanges(ref composers, value); }
        }

        public IReadOnlyList<string> Conductors
        {
            get { return conductors; }
            set { SetPropertyAndTrackChanges(ref conductors, value); }
        }


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
}
