#region usings

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

#endregion

namespace Heath.Lister.Converters
{
    public class ListSortDirectionToBitmapImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapImage retval;

            switch ((ListSortDirection)value)
            {
                case ListSortDirection.Ascending:
                    retval = new BitmapImage(new Uri("/Images/down.png", UriKind.Relative));
                    break;

                case ListSortDirection.Descending:
                    retval = new BitmapImage(new Uri("/Images/up.png", UriKind.Relative));
                    break;

                default:
                    retval = new BitmapImage(new Uri("/Images/down.png", UriKind.Relative));
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