﻿using Windows.Storage.FileProperties;

namespace Waf.MusicManager.Presentation.Services.Metadata;

internal class FlacSaveMetadata : SaveMetadata
{
    protected override void ApplyConductors(MusicProperties properties, IDictionary<string, object> customProperties, IEnumerable<string> conductors)
    {
        ApplyAsOneItem(properties.Conductors, conductors);
    }

    private static void ApplyAsOneItem(IList<string> target, IEnumerable<string> source)
    {
        // The WinRT API does not support some of the multiple tags for MP3 files; it aborts saving the metadata without error :-(
        target.Clear();
        target.Add(StringListConverter.ToString(source));
    }
}
