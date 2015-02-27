using MetaSyllabus.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Navigation;

//TODO: AlchemyAPI, Stanford/Harvard/UWaterloo attribution
namespace MetaSyllabus
{
    public partial class MainWindow : NavigationWindow
    {
        #region Constructors
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new ApplicationViewModel(this.NavigationService);
        }
        #endregion // Constructors
    }
}
