#region usings

using System;
using Microsoft.Phone.Controls;

#endregion

namespace Heath.Lister.Infrastructure
{
    public class NavigationService : INavigationService
    {
        private readonly PhoneApplicationFrame _frame;

        public NavigationService(PhoneApplicationFrame frame)
        {
            _frame = frame;
        }

        #region INavigationService Members

        public bool CanGoBack()
        {
            return _frame.CanGoBack;
        }

        public void GoBack()
        {
            _frame.GoBack();
        }

        public void Navigate(Uri source)
        {
            _frame.Navigate(source);
        }

        #endregion
    }
}