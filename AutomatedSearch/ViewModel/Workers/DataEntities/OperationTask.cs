using System;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Wpf;

namespace AutomatedSearch.Workers.DataEntities
{
    public class OperationTask : OperationBase
    {
        public Func<WebView2, Task> OnSuccess { get; set; }
        public Func<WebView2, Task> OnFail { get; set; }

        public OperationTask(Func<WebView2, Task> onSuccess, string url, Func<WebView2, Task> onFail = null)
        {
            this.OnSuccess = onSuccess;
            this.OnFail = onFail;

            base.Url = url;
        }

        public OperationTask(Func<WebView2, Task> onSuccess, string url, Int32 waitBeforeProceedTimeout, Func<WebView2, Task> onFail = null)
        {
            this.OnSuccess = onSuccess;
            this.OnFail = onFail;

            base.Url = url;

            base.WaitBeforeProceedTimeout = waitBeforeProceedTimeout;
        }

        public override void Dispose()
        {
            OnSuccess = null;
            OnFail = null;

            base.Dispose();
        }

    }
}
