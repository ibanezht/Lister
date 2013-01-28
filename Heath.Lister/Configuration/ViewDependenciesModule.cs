#region usings

using System;
using System.Windows;
using Heath.Lister.Infrastructure;
using Heath.Lister.ViewModels;
using Ninject;
using Ninject.Modules;

#endregion

namespace Heath.Lister.Configuration
{
    internal class ViewDependenciesModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<INavigationService>()
                  .ToMethod(_ => new NavigationService(((App)Application.Current).RootFrame))
                  .InSingletonScope();

            Kernel.Bind<Func<HubItemViewModel>>()
                  .ToMethod(c => () => new HubItemViewModel(c.Kernel.Get<INavigationService>()));

            Kernel.Bind<Func<ListItemViewModel>>()
                  .ToMethod(c => () => new ListItemViewModel(c.Kernel.Get<INavigationService>()));
        }
    }
}