#region usings

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#endregion

namespace Heath.Lister.Converters
{
    public class BooleanToTextValueConverter : DependencyObject, IValueConverter
    {
        private const string TrueTextPropertyName = "TrueText";
        private const string FalseTextPropertyName = "FalseText";

        public static readonly DependencyProperty TrueTextProperty =
            DependencyProperty.Register(
                TrueTextPropertyName,
                typeof(string),
                typeof(BooleanToTextValueConverter),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty FalseTextProperty =
            DependencyProperty.Register(
                FalseTextPropertyName,
                typeof(string),
                typeof(BooleanToTextValueConverter),
                new PropertyMetadata(string.Empty));

        public string TrueText
        {
            get { return (string)GetValue(TrueTextProperty); }
            set { SetValue(TrueTextProperty, value); }
        }

        public string FalseText
        {
            get { return (string)GetValue(FalseTextProperty); }
            set { SetValue(FalseTextProperty, value); }
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? TrueText : FalseText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion
    }
}