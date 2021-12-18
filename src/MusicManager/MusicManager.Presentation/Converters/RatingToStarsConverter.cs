using System;
using System.Globalization;
using System.Windows.Data;

namespace Waf.MusicManager.Presentation.Converters
{
    public class RatingToStarsConverter : IValueConverter
    {
        public object Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            int rating = System.Convert.ToInt32(value, CultureInfo.InvariantCulture);
            return rating switch
            {
                >= 99 => 5,
                >= 75 => 4,
                >= 50 => 3,
                >= 25 => 2,
                >= 1 => 1,
                _ => 0,
            };
        }

        public object ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            int stars = System.Convert.ToInt32(value, CultureInfo.InvariantCulture);
            return stars switch
            {
                5 => 99,
                4 => 75,
                3 => 50,
                2 => 25,
                1 => 1,
                _ => 0,
            };
        }
    }
}
