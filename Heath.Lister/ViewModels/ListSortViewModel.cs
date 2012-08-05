#region usings

using System.ComponentModel;

#endregion

namespace Heath.Lister.ViewModels
{
    public class ListSortViewModel
    {
        public ListSortBy ListSortBy { get; set; }

        public ListSortDirection ListSortDirection { get; set; }

        public bool HideCompleted { get; set; }
    }
}