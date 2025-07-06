using System;
using System.Windows;
using System.Windows.Threading;
using AutomatedSearch.View.Windows;
using AutomatedSearch.ViewModel.Workers;

namespace AutomatedSearch
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ViewModel.ViewModel _business;
        private MainWindow _main;

        public App()
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            this.Exit += App_Exit;

            Worker worker = new Worker();
            worker.Start(delegate ()
            {
                _business = new ViewModel.ViewModel();
                App.Current.Dispatcher.Invoke(delegate ()
                {
                    _main = new MainWindow(_business);
                    _main.Show();
                });
            });
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            object obj = new object();
            lock (obj)
            {
                MessageBoxResult res = MessageBox.Show(e.Exception.Message, "Unhandled exception occured!", MessageBoxButton.OK, MessageBoxImage.Error);
                if (res == MessageBoxResult.OK)
                {
                    Environment.Exit(-1);
                    return;
                }
            }
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            _business.Dispose();
            _main.Dispose();
        }

    }
}
