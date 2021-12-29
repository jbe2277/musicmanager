using System.Globalization;
using System.Windows.Data;
using Waf.MusicManager.Applications.Data;

namespace Waf.MusicManager.Presentation.Converters
{
    public class StringListToStringConverter : IValueConverter
    {
        public object Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture) => StringListConverter.ToString((IEnumerable<string>)value!, GetSeparator(parameter));

        public object ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture) => StringListConverter.FromString((string)value!, GetSeparator(parameter));

        private static string? GetSeparator(object? commandParameter) => ConverterHelper.IsParameterSet("ListSeparator", commandParameter) ? null : Environment.NewLine;
    }
}
