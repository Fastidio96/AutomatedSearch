using System.Windows;
using AutomatedSearch.ViewModel.Workers.TestCase;

namespace AutomatedSearch.View.Windows
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();

            this.Loaded += TestWindow_Loaded;
        }

        private void TestWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WebViewTEST test = new WebViewTEST(this);
        }
    }
}
