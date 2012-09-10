#region usings

using System;
using System.Windows;
using System.Windows.Navigation;

#endregion

namespace Heath.Lister.Configuration
{
    internal static class UriMappings
    {
        private static UriMapperBase _instance;

        public static UriMapperBase Instance
        {
            get { return _instance; }
        }

        public static void Configure()
        {
            var uriMapper = new UriMapper();

            var aboutMapping = new UriMapping
                               {
                                   Uri = new Uri("/About", UriKind.Relative),
                                   MappedUri = new Uri("/Heath.Lister.About;component/Views/AboutView.xaml", UriKind.Relative)
                               };

            var editListMapping = new UriMapping
                                  {
                                      Uri = new Uri("/EditList/{id}", UriKind.Relative),
                                      MappedUri = new Uri("/Views/EditListView.xaml?Id={id}", UriKind.Relative)
                                  };

            var listMapping = new UriMapping
                              {
                                  Uri = new Uri("/List/{id}", UriKind.Relative),
                                  MappedUri = new Uri("/Views/ListView.xaml?Id={id}", UriKind.Relative)
                              };

            var editItemMapping = new UriMapping
                                  {
                                      Uri = new Uri("/EditItem/{id}/{listId}", UriKind.Relative),
                                      MappedUri = new Uri("/Views/EditItemView.xaml?Id={id}&ListId={listId}", UriKind.Relative)
                                  };

            var itemDetailsMapping = new UriMapping
                                     {
                                         Uri = new Uri("/Item/{id}/{listId}", UriKind.Relative),
                                         MappedUri = new Uri("/Views/ItemDetailsView.xaml?Id={id}&ListId={listId}", UriKind.Relative)
                                     };

            uriMapper.UriMappings.Add(aboutMapping);
            uriMapper.UriMappings.Add(editListMapping);
            uriMapper.UriMappings.Add(listMapping);
            uriMapper.UriMappings.Add(editItemMapping);
            uriMapper.UriMappings.Add(itemDetailsMapping);

            _instance = uriMapper;

            ((App)Application.Current).RootFrame.UriMapper = uriMapper;
        }
    }
}