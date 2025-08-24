using System.Globalization;
using System.Windows.Data;
using Waf.MusicManager.Domain.Transcoding;
using Waf.MusicManager.Presentation.Properties;

namespace Waf.MusicManager.Presentation.Converters;

public class TranscodeStatusToStringConverter : IValueConverter
{
    public static TranscodeStatusToStringConverter Default { get; } = new();

    public object? Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
    {
        if (value == null) return null;
        var transcodeStatus = (TranscodeStatus)value;
        return transcodeStatus switch
        {
            TranscodeStatus.InProgress => Resources.InProgress,
            TranscodeStatus.Pending => Resources.Pending,
            TranscodeStatus.Error => Resources.Error,
            TranscodeStatus.Completed => Resources.Completed,
            _ => throw new InvalidOperationException("Enum value is unknown."),
        };
    }

    public object ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture) => throw new NotSupportedException();
}
