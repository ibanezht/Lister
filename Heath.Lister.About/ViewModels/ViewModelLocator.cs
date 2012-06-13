#region usings

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace Heath.Lister.About.ViewModels
{
    public class ViewModelLocator
    {
        private static bool? _isInDesignMode;
        private static AboutViewModel _about;

        public static bool IsInDesignMode
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
                    _isInDesignMode = DesignerProperties.IsInDesignTool;
                }

                return _isInDesignMode.Value;
            }
        }

        public static AboutViewModel AboutStatic
        {
            get
            {
                if (_about == null)
                {
                    CreateAbout();
                }

                return _about;
            }
        }

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public AboutViewModel About
        {
            get { return AboutStatic; }
        }

        public static void ClearAbout()
        {
            if (_about != null)
            {
                _about.Cleanup();
                _about = null;
            }
        }

        public static void CreateAbout()
        {
            if (_about == null)
            {
                _about = new AboutViewModel();
            }
        }

        public static void Cleanup()
        {
            ClearAbout();
        }
    }
}