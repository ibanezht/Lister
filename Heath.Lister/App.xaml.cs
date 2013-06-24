#define paidbuild
//#undef paidbuild

#region usings

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using GalaSoft.MvvmLight.Threading;
using Heath.Lister.Configuration;
using Heath.Lister.Infrastructure;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;

#endregion

namespace Heath.Lister
{
    public partial class App
    {
        public App()
        {
            UnhandledException += ApplicationUnhandledException;

            InitializeComponent();

#if paidbuild
            AppMonetizationType = AppMonetizationType.Paid;
#else
            AppMonetizationType = AppMonetizationType.Adds;
#endif

            InitializePhoneApplication();

            var radDiagnostics = new RadDiagnostics();
            radDiagnostics.EmailTo = "listerapp@hotmail.com";
            radDiagnostics.Init();

            UriMappings.Configure();

            DependencyContainer.Configure(new ViewDependenciesModule());

            DispatcherHelper.Initialize();

            using (var data = new DataAccess())
            {
                data.Initialize();
            }

            InteractionEffectManager.AllowedTypes.Add(typeof(Button));
            InteractionEffectManager.AllowedTypes.Add(typeof(RadDataBoundListBoxItem));

            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                // Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                // Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                // Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disable user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                // PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;

                // MetroGridHelper.IsVisible = true;
            }
        }

        public RadPhoneApplicationFrame RootFrame { get; private set; }

        internal static AppOpenState AppOpenState { get; set; }

        internal static AppMonetizationType AppMonetizationType { get; set; }

        internal static bool RemoveBackEntry { get; set; }

        internal static bool RemoveBackOnNext { get; set; }

        private void ApplicationLaunching(object sender, LaunchingEventArgs e)
        {
            ApplicationUsageHelper.Init("1.5");

            AppOpenState = AppOpenState.Launching;
        }

        private void ApplicationActivated(object sender, ActivatedEventArgs e)
        {
            if (!e.IsApplicationInstancePreserved)
                ApplicationUsageHelper.OnApplicationActivated();

            AppOpenState = AppOpenState.Activated;
        }

        private void ApplicationDeactivated(object sender, DeactivatedEventArgs e)
        {
            AppOpenState = AppOpenState.Deactivated;
        }

        private void ApplicationClosing(object sender, ClosingEventArgs e)
        {
            AppOpenState = AppOpenState.Closing;
        }

        private static void RootFrameNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
                Debugger.Break();
        }

        private static void ApplicationUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
                Debugger.Break();
        }

        #region Phone application initialization

        private bool _phoneApplicationInitialized;

        private void InitializePhoneApplication()
        {
            if (_phoneApplicationInitialized)
                return;

            RootFrame = new RadPhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            RootFrame.NavigationFailed += RootFrameNavigationFailed;

            _phoneApplicationInitialized = true;
        }

        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}