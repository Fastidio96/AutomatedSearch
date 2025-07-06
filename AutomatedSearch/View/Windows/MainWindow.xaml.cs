using System;
using System.Windows;
using System.Windows.Threading;
using AutomatedSearch.Model;
using AutomatedSearch.ViewModel.Workers;

namespace AutomatedSearch.View.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        public WebViewWorkerUC WebViewWorker;

        private ViewModel.ViewModel _business;

        public MainWindow(ViewModel.ViewModel business)
        {
            InitializeComponent();

            _business = business;

            Application.Current.MainWindow = this;
            this.Loaded += MainWindow_Loaded;

            WebViewWorker = webViewWorker;
            //WebViewWorker.Visibility = Visibility.Hidden; //Collapsed not works!

            dgAccounts.ItemsSource = _business.AppData.Accounts;
            _business.AppData.Accounts.CollectionChanged += Accounts_CollectionChanged;

            dgAccounts.SelectionChanged += DgAccounts_SelectionChanged;
        }

        private void Accounts_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.InvokeAsync(dgAccounts.Items.Refresh);
        }


        private void DgAccounts_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dgAccounts.SelectedItem == null)
            {
                btnRemoveAcc.IsEnabled = false;
            }
            else
            {
                btnRemoveAcc.IsEnabled = true;
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _business.InitCore(this.WebViewWorker);
            this.Loaded -= MainWindow_Loaded;
        }

        private void btnAddAcc_Click(object sender, RoutedEventArgs e)
        {
            _business.AppData.Accounts.Add(new User());
        }

        private void btnRemoveAcc_Click(object sender, RoutedEventArgs e)
        {
            if (dgAccounts.SelectedItem == null)
            {
                return;
            }

            User u = dgAccounts.SelectedItem as User;
            if (u == null)
            {
                return;
            }

            _business.RemoveAccount(u);
        }

        
        public void Dispose()
        {
            WebViewWorker.Dispose();
            dgAccounts.ItemsSource = null;
        }
    }
}
