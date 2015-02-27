using System.Windows.Controls;

namespace MetaSyllabus.Pages
{
    public partial class CourseListingsPage : Page
    {
        public CourseListingsPage()
        {
            InitializeComponent();

            gx_ZoomControl.Loaded += (sender, e) => { gx_ZoomControl.ZoomToFill(); };

            // Sometimes GraphX's ZoomControl.UpdateViewport() tries to create a Rect 
            // with negative dimensions.  Catch that error and ignore it.
            App.Current.DispatcherUnhandledException += (sender, e) => { e.Handled = true; };
        }
    }
}
