#region usings

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Heath.Lister.Localization;

#endregion

namespace Heath.Lister.Converters
{
    public class ListSortDirectionToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retval;

            switch ((ListSortDirection)value)
            {
                case ListSortDirection.Ascending:
                    retval = AppResources.AscendingText;
                    break;

                case ListSortDirection.Descending:
                    retval = AppResources.DescendingText;
                    break;

                default:
                    retval = AppResources.AscendingText;
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