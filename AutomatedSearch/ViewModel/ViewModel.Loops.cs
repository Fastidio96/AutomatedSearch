using System;
using System.Threading;
using AutomatedSearch.Model;
using AutomatedSearch.ViewModel.Helpers;
using AutomatedSearch.ViewModel.Workers;

namespace AutomatedSearch.ViewModel
{
    public partial class ViewModel
    {
        private Thread _worker;
        private bool _workerCoreRunning;

        private WebViewWorkerUC _workerUC;

        public void InitCore(WebViewWorkerUC workerUC)
        {
            _workerUC = workerUC;
            _workerCoreRunning = true;

            if (_worker == null)
            {
                _worker = new Thread(WorkerCoreLoop);
                _worker.Name = "CoreLoop worker";
                _worker.Start();
            }
        }

        private void WorkerCoreLoop()
        {
            while (!_workerUC.IsReady)
            {
                Thread.Sleep(100);
            }

            while (_workerCoreRunning)
            {
                foreach (User account in AppData.Accounts)
                {
                    AppData.CurrentUser = account;

                    if (DateTimeUtilities.HasElapsed(DateTime.UtcNow, AppData.CurrentUser.LastUpdate, new TimeSpan(12, 0, 0)) || true)
                    {
                        if (IsUserLogged(_workerUC))
                        {
                            if (AppData.CurrentUser.Cookies == null || AppData.CurrentUser.Cookies.Count == 0)
                            {
                                GatherCookies(_workerUC.WebView);
                            }

                            LoadCookies(_workerUC.WebView);
                        }
                        else
                        {
                            Login();

                            while (AppData.CurrentUser.Status != UserStatus.Logged)
                            {
                                Thread.Sleep(500); // Lock loop and wait
                            }
                        }

                        GatherUserInfo(_workerUC);

                        Int32 todoSearches = AppData.CurrentUser.MaxDailySearch - AppData.CurrentUser.CurrentDailySearch;
                        if (todoSearches > 0)
                        {
                            DoSearches(_workerUC, todoSearches, AppData.CurrentUser);
                        }
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private void DoSearches(WebViewWorkerUC workerUC, Int32 todoSearches, User account)
        {
            Random rnd = new Random();
            for (int i = 0; i < todoSearches; i++)
            {
                if (_disposing)
                {
                    break;
                }

                DoSearch(workerUC);
                account.CurrentDailySearch = +1;

                Thread.Sleep(rnd.Next(2500, 5000));
            }

            GatherUserInfo(workerUC); //On finish reload infos
        }

    }
}
