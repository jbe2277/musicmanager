using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Waf.MusicManager.Presentation.Converters;

public class PathToDirectoryNameConverter : IValueConverter
{
    public static PathToDirectoryNameConverter Default { get; } = new();

    public object? Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture) => Path.GetDirectoryName(value as string);

    public object ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture) => throw new NotSupportedException();
}
