#region usings

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Heath.Lister.Infrastructure.Models;
using Heath.Lister.Localization;

#endregion

namespace Heath.Lister.Converters
{
    public class PriorityToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string retval;

            switch ((Priority)value)
            {
                case Priority.High:
                    retval = AppResources.HighText;
                    break;

                case Priority.Medium:
                    retval = AppResources.MediumText;
                    break;

                case Priority.Low:
                    retval = AppResources.LowText;
                    break;

                default:
                    retval = AppResources.NoneText;
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