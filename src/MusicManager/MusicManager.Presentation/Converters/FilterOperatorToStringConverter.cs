using System.Globalization;
using System.Windows.Data;
using Waf.MusicManager.Applications.DataModels;
using Waf.MusicManager.Presentation.Properties;

namespace Waf.MusicManager.Presentation.Converters;

public class FilterOperatorToStringConverter : IValueConverter
{
    public static FilterOperatorToStringConverter Default { get; } = new();

    public object? Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
    {
        if (value == null) return null;
        var isDescription = ConverterHelper.IsParameterSet("description", parameter);
        var filterOperator = (FilterOperator)value;
        return filterOperator switch
        {
            FilterOperator.Ignore => isDescription ? Resources.IgnoreValue : "",
            FilterOperator.LessThanOrEqual => isDescription ? Resources.LessThanOrEqual : "<=",
            FilterOperator.GreaterThanOrEqual => isDescription ? Resources.GreaterThanOrEqual : ">=",
            _ => throw new InvalidOperationException("Enum value is unknown."),
        };
    }

    public object ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture) => throw new NotSupportedException();
}
