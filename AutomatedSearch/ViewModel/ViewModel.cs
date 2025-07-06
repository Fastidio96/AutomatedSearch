using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using AutomatedSearch.Model;
using AutomatedSearch.View.Windows;
using AutomatedSearch.ViewModel.Helpers;
using AutomatedSearch.ViewModel.Workers;
using AutomatedSearch.Workers.DataEntities;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace AutomatedSearch.ViewModel
{
    public partial class ViewModel : IDisposable
    {
        public AppData AppData { get; set; }

        private bool disposedValue;
        private bool _disposing;

        public ViewModel()
        {
            AppData = new AppData();
            LoadFromFile();

            AppData.PropertyChanged += AppData_PropertyChanged;
        }

        private void AppData_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("Updated property {0}", e.PropertyName));
            SaveToFile();
        }

        public void SendMessage(string message)
        {
            Dispatcher.CurrentDispatcher.InvokeAsync(delegate
            {
                MessageBox.Show(App.Current.MainWindow, message, "Automated search", MessageBoxButton.OK);
            });
        }

        public void SendMessage(Exception ex)
        {
            Dispatcher.CurrentDispatcher.InvokeAsync(delegate
            {
                MessageBox.Show(App.Current.MainWindow, ex.Message, "Unhandled exception occured!", MessageBoxButton.OK);
            });
        }

        public T AwaitOperation<T>(Func<T> action) where T : IComparable
        {
            bool canContinue = false;

            T res = action.Invoke();

            while (!canContinue && !_disposing) //WaitAsync the worker
            {
                Thread.Sleep(50);
            }

            return res;
        }

        public async void AwaitOperationDispatcher(Action action)
        {
            bool canContinue = false;

            await App.Current.Dispatcher.InvokeAsync(delegate ()
            {
                action.Invoke();
                canContinue = true;
            });

            while (!canContinue && !_disposing) //WaitAsync the worker
            {
                Thread.Sleep(50);
            }
        }

        public async void AwaitOperationDispatcher(Func<Task> action)
        {
            bool canContinue = false;

            await App.Current.Dispatcher.InvokeAsync(async delegate ()
            {
                await action.Invoke().ContinueWith(t => canContinue = true);
            });

            while (!canContinue && !_disposing) //WaitAsync the worker
            {
                Thread.Sleep(50);
            }
        }

        public bool IsUserLogged(WebViewWorkerUC worker)
        {
            worker.StartAndWait
            (
                new Operation[]
                {
                    new Operation
                    (
                        delegate (WebView2 webView)
                        {
                            LoadCookies(webView);
                            return true;
                        }, null
                    ),
                    new Operation
                    (
                        delegate (WebView2 webView) //No redirect
                        {
                            AppData.CurrentUser.Status = UserStatus.Logged;
                            return true;
                        }, Costants.URL_REWARDS,
                        delegate (WebView2 webView) //Redirect to login detected
                        {
                            AppData.CurrentUser.Status = UserStatus.NotLogged;
                            return true;
                        }
                    )
                }
            );

            return AppData.CurrentUser.Status == UserStatus.Logged;
        }

        public void GatherCookies(WebView2 webView)
        {
            AwaitOperationDispatcher(async delegate ()
            {
                List<CoreWebView2Cookie> webviewCoookies = await webView.CoreWebView2.CookieManager.GetCookiesAsync(Costants.URL_LOGIN);

                if (webviewCoookies == null || webviewCoookies.Count == 0)
                {
                    return;
                }

                webviewCoookies.ForEach(c => AppData.CurrentUser.Cookies.Add(new Cookie(c.Name, c.Value, c.Path, c.Domain)));
            });
        }

        public void LoadCookies(WebView2 webView)
        {
            AwaitOperationDispatcher(delegate ()
            {
                if (webView.CoreWebView2 != null)
                {
                    webView.CoreWebView2.Profile.CookieManager.DeleteAllCookies();

                    foreach (Cookie cookie in AppData.CurrentUser.Cookies.GetAllCookies())
                    {
                        webView.CoreWebView2.Profile.CookieManager.CreateCookieWithSystemNetCookie(cookie);
                    }
                }
            });
        }

        public void GatherUserInfo(WebViewWorkerUC workerUC)
        {
            if (_disposing)
            {
                return;
            }

            workerUC.StartAndWait(new OperationTask[]
            {
                new OperationTask(async delegate (WebView2 webView)
                {
                    string js = PrepareJsPathForGet(Costants.JS_PATH_ADDITIONAL_INFO);
                    string name = await webView.CoreWebView2.ExecuteScriptAsync(js);

                    AppData.CurrentUser.Username = name.Replace("\"", " ").Trim();


                    js = PrepareJsPathForGet(Costants.JS_PATH_BALANCE_POINTS);
                    string points = await webView.CoreWebView2.ExecuteScriptAsync(js);

                    points = points.Replace("\"", " ").Trim();

                    if (points.Contains('.'))
                    {
                        string[] pts = points.Split('.');
                        points = pts[0] + pts[1];
                    }

                    if (Int32.TryParse(points, out Int32 tmpPts))
                    {
                        AppData.CurrentUser.TotalPoints = tmpPts;
                    }

                }, Costants.URL_REWARDS, 500),
                new OperationTask(async delegate (WebView2 webView)
                {
                    string js = PrepareJsPathForGet(Costants.JS_PATH_SEARCHES);
                    string searches = await webView.CoreWebView2.ExecuteScriptAsync(js);

                    searches = searches.Replace("\"", " ").Trim();
                    string[] tmpSearch = searches.Split("/");


                    if (Int32.TryParse(tmpSearch[0].Trim(), out Int32 curSearch))
                    {
                        AppData.CurrentUser.CurrentDailySearch = curSearch;
                    }

                    if (Int32.TryParse(tmpSearch[1].Trim(), out Int32 mxSrc))
                    {
                        AppData.CurrentUser.MaxDailySearch = mxSrc;
                    }

                    AppData.CurrentUser.LastUpdate = DateTime.UtcNow;
                }, Costants.URL_POINTS_INFOS)
            });
        }

        public void DoSearch(WebViewWorkerUC workerUC)
        {
            string url = string.Format("{0}{1}", Costants.URL_SEARCH_FORMAT, WordGenerator.GetRandomString());

            Random rnd = new Random();
            workerUC.StartAndWait(new Operation(null, url, rnd.Next(3500, 5000)));
        }

        public void Login()
        {
            App.Current?.Dispatcher.Invoke(delegate ()
            {
                LoginWindow loginWindow = new LoginWindow(this);
                loginWindow.Owner = App.Current.MainWindow;
                loginWindow.Show();
            });
        }

        public void Logout(WebViewWorkerUC workerUC)
        {
            if (!IsUserLogged(workerUC))
            {
                return;
            }

            workerUC.Start(new Operation(delegate (WebView2 webView)
            {
                AppData.CurrentUser = null;
                return true;
            }, Costants.URL_LOGOUT));
        }

#if DEBUG

        public void Test()
        {
            App.Current.Dispatcher.BeginInvoke(() =>
            {
                TestWindow test = new TestWindow();
                test.Owner = App.Current.MainWindow;
                test.Show();
            });
        }

#endif


        #region Helpers

        private string PrepareJsPathForGet(string jsPath)
        {
            return string.Format("document.querySelector(\"{0}\")?.innerText.trim();", jsPath);
        }

        #endregion

        #region IDisposable support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _disposing = true;

                    AppData.PropertyChanged -= AppData_PropertyChanged;

                    AppData.CurrentUser = null;
                    _workerCoreRunning = false;

                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
