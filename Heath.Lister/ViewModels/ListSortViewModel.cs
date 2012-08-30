#region usings

using System.ComponentModel;

#endregion

namespace Heath.Lister.ViewModels
{
    public class ListSortViewModel
    {
        public bool HideCompleted { get; set; }

        public ListSortBy ListSortBy { get; set; }

        public ListSortDirection ListSortDirection { get; set; }
    }
}