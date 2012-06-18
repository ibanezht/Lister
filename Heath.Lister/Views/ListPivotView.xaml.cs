#region usings

using Telerik.Windows.Controls;

#endregion

namespace Heath.Lister.Views
{
    public partial class ListPivotView
    {
        public ListPivotView()
        {
            InitializeComponent();
        }

        private void ListItemsRadDataBoundListBoxItemTap(object sender, ListBoxItemTapEventArgs e)
        {
            var listView = ElementTreeHelper.FindVisualAncestor<ListView>(this);
            
            listView.SetValue(RadTileAnimation.ElementToDelayProperty, e.Item);
        }
    }
}