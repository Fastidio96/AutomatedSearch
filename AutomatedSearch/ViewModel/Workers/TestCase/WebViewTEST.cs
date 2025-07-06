using System;
using System.Threading;
using System.Windows;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace AutomatedSearch.ViewModel.Workers.TestCase
{
    public class WebViewTEST
    {
        public IntPtr HWND { get; private set; }

        private Window _window;

        public WebViewTEST(Window window)
        {
            _window = window;

            Init();
        }

        private async void Init()
        {
            CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions("--disable-web-security");
            CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(null, null, options);

            WebView2 webView = new WebView2();

            CancellationToken token = new CancellationToken();
            await webView.EnsureCoreWebView2Async(environment).WaitAsync(token);

            //if (webView.CoreWebView2 == null)
            //{
            //    Thread.Sleep(50);
            //}

            webView.Visibility = Visibility.Hidden;
            webView.NavigationCompleted += WebView_NavigationCompleted;
            webView.CoreWebView2.Navigate("https://www.google.com");
        }

        private void WebView_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            MessageBox.Show("DONE");
        }
    }
}
