#region usings

using System.ComponentModel;
using Heath.Lister.Infrastructure;

#endregion

namespace Heath.Lister.ViewModels
{
    public class ListSortViewModel
    {
        private readonly Setting<bool> _hideCompletedSetting
            = new Setting<bool>("HideCompleted", false);

        private readonly Setting<ListSortBy> _listSortBySetting
            = new Setting<ListSortBy>("ListSortBy", ListSortBy.Due);

        private readonly Setting<ListSortDirection> _listSortDirectionSetting
            = new Setting<ListSortDirection>("ListSortDirection", ListSortDirection.Ascending);

        public ListSortViewModel()
        {
            HideCompleted = _hideCompletedSetting.Value;
            ListSortBy = _listSortBySetting.Value;
            ListSortDirection = _listSortDirectionSetting.Value;
        }

        public bool HideCompleted { get; set; }

        public ListSortBy ListSortBy { get; set; }

        public ListSortDirection ListSortDirection { get; set; }

        public void Persist()
        {
            _hideCompletedSetting.Value = HideCompleted;
            _listSortBySetting.Value = ListSortBy;
            _listSortDirectionSetting.Value = ListSortDirection;
        }
    }
}