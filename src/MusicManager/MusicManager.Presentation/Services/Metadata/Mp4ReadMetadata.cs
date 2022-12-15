using Windows.Storage.FileProperties;

namespace Waf.MusicManager.Presentation.Services.Metadata;

internal class Mp4ReadMetadata : ReadMetadata
{
    protected override IEnumerable<string> ReadGenre(MusicProperties properties, IDictionary<string, object> customProperties)
    {
        return TryParseFromOneItem(base.ReadGenre(properties, customProperties));
    }

    private static IReadOnlyList<string> TryParseFromOneItem(IEnumerable<string> source)
    {
        // The WinRT API does not support some of the multiple tags for MP4 files.
        return source.Count() == 1 ? StringListConverter.FromString(source.First()) : source.ToArray();
    }
}
