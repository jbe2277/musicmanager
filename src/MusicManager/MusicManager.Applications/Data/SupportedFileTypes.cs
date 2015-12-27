using System;
using System.Collections.Generic;
using System.Linq;
using Waf.MusicManager.Applications.Data.Metadata;

namespace Waf.MusicManager.Applications.Data
{
    internal static class SupportedFileTypes
    {
        private static readonly string[] musicFileExtensions = new string[] { ".mp3", ".wma", ".wav", ".m4a", ".mp4" };
        private static readonly string[] playlistFileExtensions = new string[] { ".m3u", ".wpl" };

        private static readonly Mp3ReadMetadata mp3ReadMetadata = new Mp3ReadMetadata();
        private static readonly WmaReadMetadata wmaReadMetadata = new WmaReadMetadata();
        private static readonly AacReadMetadata aacReadMetadata = new AacReadMetadata();
        private static readonly WavReadMetadata wavReadMetadata = new WavReadMetadata();
        private static readonly Mp4ReadMetadata mp4ReadMetadata = new Mp4ReadMetadata();
        private static readonly FlacReadMetadata flacReadMetadata = new FlacReadMetadata();

        private static readonly Mp3SaveMetadata mp3SaveMetadata = new Mp3SaveMetadata();
        private static readonly WmaSaveMetadata wmaSaveMetadata = new WmaSaveMetadata();
        private static readonly AacSaveMetadata aacSaveMetadata = new AacSaveMetadata();
        private static readonly WavSaveMetadata wavSaveMetadata = new WavSaveMetadata();
        private static readonly Mp4SaveMetadata mp4SaveMetadata = new Mp4SaveMetadata();
        private static readonly FlacSaveMetadata flacSaveMetadata = new FlacSaveMetadata();


        public static IReadOnlyList<string> MusicFileExtensions { get { return AddFlacExtensionWhenSupported(musicFileExtensions); } }

        public static IReadOnlyList<string> PlaylistFileExtensions { get { return playlistFileExtensions; } }

        private static bool IsFlacSupported
        {
            get { return Environment.OSVersion.Version.Major >= 10; }
        }


        internal static ReadMetadata GetReadMetadata(string fileExtension)
        {
            if (fileExtension.Equals(".mp3", StringComparison.OrdinalIgnoreCase))
            {
                return mp3ReadMetadata;
            }
            else if (fileExtension.Equals(".wma", StringComparison.OrdinalIgnoreCase))
            {
                return wmaReadMetadata;
            }
            else if (fileExtension.Equals(".m4a", StringComparison.OrdinalIgnoreCase))
            {
                return aacReadMetadata;
            }
            else if (fileExtension.Equals(".wav", StringComparison.OrdinalIgnoreCase))
            {
                return wavReadMetadata;
            }
            else if (fileExtension.Equals(".mp4", StringComparison.OrdinalIgnoreCase))
            {
                return mp4ReadMetadata;
            }
            else if (IsFlacSupported && fileExtension.Equals(".flac", StringComparison.OrdinalIgnoreCase))
            {
                return flacReadMetadata;
            }
            else
            {
                throw new NotSupportedException("The provided extension '" + fileExtension + "' is not supported.");
            }
        }

        internal static SaveMetadata GetSaveMetadata(string fileExtension)
        {
            if (fileExtension.Equals(".mp3", StringComparison.OrdinalIgnoreCase))
            {
                return mp3SaveMetadata;
            }
            else if (fileExtension.Equals(".wma", StringComparison.OrdinalIgnoreCase))
            {
                return wmaSaveMetadata;
            }
            else if (fileExtension.Equals(".m4a", StringComparison.OrdinalIgnoreCase))
            {
                return aacSaveMetadata;
            }
            else if (fileExtension.Equals(".wav", StringComparison.OrdinalIgnoreCase))
            {
                return wavSaveMetadata;
            }
            else if (fileExtension.Equals(".mp4", StringComparison.OrdinalIgnoreCase))
            {
                return mp4SaveMetadata;
            }
            else if (IsFlacSupported && fileExtension.Equals(".flac", StringComparison.OrdinalIgnoreCase))
            {
                return flacSaveMetadata;
            }
            else
            {
                throw new NotSupportedException("The provided extension '" + fileExtension + "' is not supported.");
            }
        }

        private static IReadOnlyList<string> AddFlacExtensionWhenSupported(IReadOnlyList<string> extensions)
        {
            if (IsFlacSupported)
            {
                return extensions.Concat(new[] { ".flac" }).ToArray();
            }
            return extensions;
        }
    }
}
