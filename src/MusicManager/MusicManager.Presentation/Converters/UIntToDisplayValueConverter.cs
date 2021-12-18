﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Waf.MusicManager.Presentation.Converters
{
    public class UIntToDisplayValueConverter : IValueConverter
    {
        public object Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            var number = (uint)value!;
            return number != 0 ? number : "";
        }

        public object ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            var displayValue = value as string;
            return string.IsNullOrEmpty(displayValue) ? 0 : uint.Parse(displayValue, CultureInfo.CurrentCulture);
        }
    }
}
