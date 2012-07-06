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
                           : DependencyContainer.Instance.Get<HubViewModel>();
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
                           ? new ListViewModel(null, null)
                           : DependencyContainer.Instance.Get<ListViewModel>();
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
                           : DependencyContainer.Instance.Get<EditListViewModel>();
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
                           : DependencyContainer.Instance.Get<EditItemViewModel>();
            }
        }

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditItemViewModel EditItem
        {
            get { return EditItemStatic; }
        }

        public static EditReminderViewModel EditReminderStatic
        {
            get
            {
                return IsInDesignMode
                           ? new EditReminderViewModel(null)
                           : DependencyContainer.Instance.Get<EditReminderViewModel>();
            }
        }

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public EditReminderViewModel EditReminder
        {
            get { return EditReminderStatic; }
        }

        public static ItemDetailsViewModel ItemDetailsStatic
        {
            get
            {
                return IsInDesignMode
                           ? new ItemDetailsViewModel(null)
                           : DependencyContainer.Instance.Get<ItemDetailsViewModel>();
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