#region usings

using System;

#endregion

namespace Heath.Lister.Infrastructure
{
    public interface INavigationService
    {
        bool CanGoBack();
        void GoBack();
        void Navigate(Uri source);
        void RemoveBackEntry();
    }
}