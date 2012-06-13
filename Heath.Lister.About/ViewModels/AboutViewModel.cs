#region usings

using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Heath.Lister.About.Localization;
using Microsoft.Phone.Tasks;

#endregion

namespace Heath.Lister.About.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        private string _applicationAuthor;
        private string _applicationDescription;
        private string _applicationPublisher;
        private string _applicationTitle;
        private string _applicationVersion;

        public AboutViewModel()
        {
            PageName = AppResources.AboutPageName;

            MoreCommand = new RelayCommand(More);
            ReviewCommand = new RelayCommand(Review);

            LoadManifestData();
        }

        public string PageName { get; private set; }

        public string ApplicationAuthor
        {
            get { return _applicationAuthor; }
            set
            {
                _applicationAuthor = value;
                RaisePropertyChanged("ApplicationAuthor");
            }
        }

        public string ApplicationDescription
        {
            get { return _applicationDescription; }
            set
            {
                _applicationDescription = value;
                RaisePropertyChanged("ApplicationDescription");
            }
        }

        public string ApplicationPublisher
        {
            get { return _applicationPublisher; }
            set
            {
                _applicationPublisher = value;
                RaisePropertyChanged("ApplicationPublisher");
            }
        }

        public string ApplicationTitle
        {
            get { return _applicationTitle; }
            set
            {
                _applicationTitle = value;
                RaisePropertyChanged("ApplicationTitle");
            }
        }

        public string ApplicationVersion
        {
            get { return _applicationVersion; }
            set
            {
                _applicationVersion = value;
                RaisePropertyChanged("ApplicationVersion");
            }
        }

        public ICommand MoreCommand { get; private set; }

        public ICommand ReviewCommand { get; private set; }

        private void LoadManifestData()
        {
            var manifest = new ManifestAppInfo();

            ApplicationTitle = manifest.Title.ToUpper();
            ApplicationDescription = manifest.Description;
            ApplicationVersion = manifest.Version;
            ApplicationAuthor = manifest.Author;
            ApplicationPublisher = manifest.Publisher;
        }

        private static void Review()
        {
            var task = new MarketplaceReviewTask();
            task.Show();
        }

        private static void More()
        {
            var t = new MarketplaceSearchTask();
            t.ContentType = MarketplaceContentType.Applications;
            t.SearchTerms = "Heath Turnage";
            t.Show();
        }
    }
}