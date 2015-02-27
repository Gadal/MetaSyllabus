using System.Windows.Controls;

namespace MetaSyllabus.Views
{
    /// <summary>
    /// A WPF list view that will scroll to its selected item automatically whenever its SelectedItem property changes.
    /// </summary>
    public class AutoScrollListView : ListView
    {
        public AutoScrollListView() : base()
        {
            SelectionChanged += new SelectionChangedEventHandler(AutoScrollListView_SelectionChanged);
        }

        private void AutoScrollListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ScrollIntoView(SelectedItem);
        }
    }
}
