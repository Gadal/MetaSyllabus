using System;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MetaSyllabus.Pages
{
    public partial class SearchPage : Page
    {
        public SearchPage()
        {
            InitializeComponent();
        }

        #region Private Methods

        // Not MVVM, but simple.  Refactor if necessary.

        private void NavToCourseListings(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("Views/Pages/CourseListingsPage.xaml", UriKind.Relative));
        }
        
        private void NavToConceptListings(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("Views/Pages/ConceptListingsPage.xaml", UriKind.Relative));
        }

        #endregion // Private Methods
    }
}
