#region usings

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Heath.Lister.Infrastructure;

#endregion

namespace Heath.Lister.Converters
{
    public class EnumValuesConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? null : EnumValueCache.GetValues(value.GetType());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion
    }
}