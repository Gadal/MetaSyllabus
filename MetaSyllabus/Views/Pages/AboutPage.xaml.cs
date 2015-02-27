using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Navigation;

namespace MetaSyllabus.Views.Pages
{
    /// <summary>
    /// Interaction logic for AboutPage.xaml
    /// </summary>
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
            metacademy_link.Click += (sender, e) => { Process.Start("http://www.metacademy.org/about"); };
            alchemyAPI_link.Click += (sender, e) => { Process.Start("http://www.alchemyapi.com");       };
        }
    }
}
