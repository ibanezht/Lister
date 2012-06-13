#region usings

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

#endregion

namespace Heath.Lister.Converters
{
    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? new SolidColorBrush((Color)(value)) : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion
    }
}