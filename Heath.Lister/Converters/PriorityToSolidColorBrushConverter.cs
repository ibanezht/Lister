#region usings

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Heath.Lister.Infrastructure.Models;
using Color = System.Windows.Media.Color;

#endregion

namespace Heath.Lister.Converters
{
    public class PriorityToSolidColorBrushConverter : DependencyObject, IValueConverter
    {
        private const string DefaultColorPropertyName = "DefaultColor";

        public static readonly DependencyProperty DefaultColorProperty =
            DependencyProperty.Register(
                DefaultColorPropertyName,
                typeof(Color),
                typeof(PriorityToSolidColorBrushConverter),
                null);

        public Color DefaultColor
        {
            get { return (Color)GetValue(DefaultColorProperty); }
            set { SetValue(DefaultColorProperty, value); }
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush retval;

            switch ((Priority)value)
            {
                case Priority.High:
                    retval = new SolidColorBrush(Color.FromArgb(255, 229, 20, 0));
                    break;

                case Priority.Medium:
                    retval = new SolidColorBrush(Color.FromArgb(255, 240, 150, 9));
                    break;

                case Priority.Low:
                    retval = new SolidColorBrush(Color.FromArgb(255, 27, 161, 226));
                    break;

                default:
                    retval = new SolidColorBrush(DefaultColor);
                    break;
            }

            return retval;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion
    }
}