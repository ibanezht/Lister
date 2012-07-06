#region usings

using System;
using GalaSoft.MvvmLight;
using Heath.Lister.Infrastructure;
using Heath.Lister.Infrastructure.ViewModels;

#endregion

namespace Heath.Lister.ViewModels
{
    public class EditReminderViewModel : ViewModelBase, IHaveListId, IPageViewModel
    {
        private const string IdPropertyName = "Id";
        private const string ListIdPropertyName = "ListId";

        private readonly INavigationService _navigationService;

        private Guid _id;
        private Guid _listId;

        public EditReminderViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            ApplicationTitle = "LISTER";
            PageName = "edit reminder";
        }

        public string ApplicationTitle { get; private set; }

        public string PageName { get; private set; }

        #region IHaveListId Members

        public Guid Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged(IdPropertyName);
            }
        }

        public Guid ListId
        {
            get { return _listId; }
            set
            {
                _listId = value;
                RaisePropertyChanged(ListIdPropertyName);
            }
        }

        #endregion

        #region IPageViewModel Members

        public void Activate() {}

        public void Deactivate(bool isNavigationInitiator) {}

        public void ViewReady() {}

        #endregion
    }
}