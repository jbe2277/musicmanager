using Waf.MusicManager.Applications.Services;
using Waf.MusicManager.Presentation.Services.Metadata;

namespace Waf.MusicManager.Presentation.Services;

internal static class SupportedFileTypes
{
    private static readonly string[] musicFileExtensions = [ ".mp3", ".wma", ".wav", ".m4a", ".mp4" ];

    private static readonly Mp3ReadMetadata mp3ReadMetadata = new();
    private static readonly WmaReadMetadata wmaReadMetadata = new();
    private static readonly AacReadMetadata aacReadMetadata = new();
    private static readonly WavReadMetadata wavReadMetadata = new();
    private static readonly Mp4ReadMetadata mp4ReadMetadata = new();
    private static readonly FlacReadMetadata flacReadMetadata = new();
    private static readonly MkvReadMetadata mkvReadMetadata = new();

    private static readonly Mp3SaveMetadata mp3SaveMetadata = new();
    private static readonly WmaSaveMetadata wmaSaveMetadata = new();
    private static readonly AacSaveMetadata aacSaveMetadata = new();
    private static readonly WavSaveMetadata wavSaveMetadata = new();
    private static readonly Mp4SaveMetadata mp4SaveMetadata = new();
    private static readonly FlacSaveMetadata flacSaveMetadata = new();
    private static readonly MkvSaveMetadata mkvSaveMetadata = new();

    public static IReadOnlyList<string> MusicFileExtensions => AddMoreExtensionsWhenSupported(musicFileExtensions);

    public static IReadOnlyList<string> PlaylistFileExtensions => IFileService.PlaylistFileExtensions;

    private static bool IsFlacSupported => Environment.OSVersion.Version.Major >= 10;

    private static bool IsMkvSupported => Environment.OSVersion.Version.Major >= 10;

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
        else if (IsMkvSupported && fileExtension.Equals(".mkv", StringComparison.OrdinalIgnoreCase))
        {
            return mkvReadMetadata;
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
        else if (IsMkvSupported && fileExtension.Equals(".mkv", StringComparison.OrdinalIgnoreCase))
        {
            return mkvSaveMetadata;
        }
        else
        {
            throw new NotSupportedException("The provided extension '" + fileExtension + "' is not supported.");
        }
    }

    private static IReadOnlyList<string> AddMoreExtensionsWhenSupported(IReadOnlyList<string> extensions)
    {
        IEnumerable<string> result = extensions;
        if (IsFlacSupported) result = result.Concat([ ".flac" ]);
        if (IsMkvSupported) result = result.Concat([ ".mkv" ]);
        return result.ToArray();
    }
}
