using System.Globalization;
using System.Windows.Data;
using Waf.MusicManager.Domain.MusicFiles;

namespace Waf.MusicManager.Presentation.Converters;

public class MusicPropertiesEnabledConverter : IMultiValueConverter
{
    public static MusicPropertiesEnabledConverter Default { get; } = new();

    public object? Convert(object?[] values, Type? targetType, object? parameter, CultureInfo? culture)
    {
        if (values[0] is not MusicFile musicFile) return null;
        // values[1] = musicFile.IsMetadataLoaded; only used to update the Binding
        return musicFile != null && musicFile.IsMetadataLoaded && musicFile.Metadata.IsSupported;
    }

    public object[] ConvertBack(object? value, Type[]? targetTypes, object? parameter, CultureInfo? culture) => throw new NotSupportedException();
}
