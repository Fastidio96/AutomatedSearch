using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using AutomatedSearch.Model;
using AutomatedSearch.Workers.DataEntities;
using Microsoft.Web.WebView2.Core;

namespace AutomatedSearch.View.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private ViewModel.ViewModel _business;

        public LoginWindow(ViewModel.ViewModel business)
        {
            InitializeComponent();

            _business = business;

            this.Closing += LoginWindow_Closing;
            webViewWorker.InitCompleted += WebViewWorker_InitCompleted;
        }

        private void WebViewWorker_InitCompleted(object? sender, EventArgs e)
        {
            webViewWorker.Start(new Operation(null, Costants.URL_LOGIN));
            webViewWorker.WebView.NavigationCompleted += WebView_NavigationCompleted;
        }

        private void WebView_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess && (webViewWorker.WebView.Source.Host == Costants.URL_HOST_LOGGED))
            {
                this.Closing -= LoginWindow_Closing;
                GatherCookies();
            }
            else
            {
                this.Visibility = Visibility.Visible;
            }
        }

        private void LoginWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true; //Need to gather cookies before exiting
            GatherCookies();
        }

        private async void GatherCookies()
        {
            List<CoreWebView2Cookie> webviewCoookies = await this.webViewWorker.WebView.CoreWebView2.CookieManager.GetCookiesAsync(Costants.URL_LOGIN);

            if (webviewCoookies == null || webviewCoookies.Count == 0)
            {
                return;
            }

            foreach (CoreWebView2Cookie c in webviewCoookies)
            {
                _business.AppData.CurrentUser.Cookies.Add(c.ToSystemNetCookie());
            }

            this.Closing -= LoginWindow_Closing;
            this.Close();
            webViewWorker.Dispose();

            //_business.AppData.CurrentUser.LastDBChange = DateTime.UtcNow; // Trigger save
            _business.AppData.CurrentUser.Status = UserStatus.Logged;
        }
    }
}
