#region usings

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#endregion

namespace Heath.Lister.Converters
{
    public class ValueEqualToVisibilityConverter : DependencyObject, IValueConverter
    {
        private const string ValuePropertyName = "Value";

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(ValuePropertyName, typeof(object), typeof(ValueEqualToVisibilityConverter), null);

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(Value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion
    }
}