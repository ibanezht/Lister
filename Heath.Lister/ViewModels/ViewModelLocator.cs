#region usings

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Heath.Lister.Infrastructure;
using Ninject;

#endregion

namespace Heath.Lister.ViewModels
{
    public class ViewModelLocator
    {
        private static bool? _isInDesignMode;

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

        public static HubViewModel HubStatic
        {
            get
            {
                return IsInDesignMode
                           ? new HubViewModel(null, null)
                           : ListerContainer.Instance.Get<HubViewModel>();
            }
        }

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public HubViewModel Hub
        {
            get { return HubStatic; }
        }

        public static ListViewModel ListStatic
        {
            get
            {
                return IsInDesignMode
                           ? new ListViewModel(null, null, null)
                           : ListerContainer.Instance.Get<ListViewModel>();
            }
        }

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ListViewModel List
        {
            get { return ListStatic; }
        }

        public static EditListViewModel EditListStatic
        {
            get
            {
                return IsInDesignMode
                           ? new EditListViewModel(null)
                           : ListerContainer.Instance.Get<EditListViewModel>();
            }
        }

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditListViewModel EditList
        {
            get { return EditListStatic; }
        }

        public static EditItemViewModel EditItemStatic
        {
            get
            {
                return IsInDesignMode
                           ? new EditItemViewModel(null)
                           : ListerContainer.Instance.Get<EditItemViewModel>();
            }
        }

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditItemViewModel EditItem
        {
            get { return EditItemStatic; }
        }

        public static ItemDetailsViewModel ItemDetailsStatic
        {
            get
            {
                return IsInDesignMode
                           ? new ItemDetailsViewModel(null)
                           : ListerContainer.Instance.Get<ItemDetailsViewModel>();
            }
        }

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ItemDetailsViewModel ItemDetails
        {
            get { return ItemDetailsStatic; }
        }
    }
}