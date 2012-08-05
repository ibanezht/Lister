#region usings

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Heath.Lister.Localization;
using Heath.Lister.ViewModels;

#endregion

namespace Heath.Lister.Converters
{
    public class ListSortByToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retval;

            switch ((ListSortBy)value)
            {
                case ListSortBy.Due:
                    retval = AppResources.DueText;
                    break;

                case ListSortBy.Title:
                    retval = AppResources.TitleText;
                    break;

                case ListSortBy.Priority:
                    retval = AppResources.PriorityText;
                    break;

                default:
                    retval = AppResources.DueText;
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