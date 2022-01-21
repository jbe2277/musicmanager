﻿using Windows.Storage.FileProperties;

namespace Waf.MusicManager.Presentation.Services.Metadata;

internal class FlacReadMetadata : ReadMetadata
{
    protected override IEnumerable<string> ReadConductors(MusicProperties properties, IDictionary<string, object> customProperties)
    {
        return TryParseFromOneItem(base.ReadConductors(properties, customProperties));
    }

    private static IEnumerable<string> TryParseFromOneItem(IEnumerable<string> source)
    {
        // The WinRT API does not support some of the multiple tags for MP3 files.
        return source.Count() == 1 ? StringListConverter.FromString(source.First()) : source.ToArray();
    }
}
