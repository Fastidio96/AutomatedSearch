using System.Threading;

namespace AutomatedSearch.ViewModel.Helpers
{
    public class ThreadUtilities
    {
        public static bool IsDispatcherThread
        {
            get => Thread.CurrentThread.GetApartmentState() == ApartmentState.STA ||
                Thread.CurrentThread.ManagedThreadId == App.Current.Dispatcher.Thread.ManagedThreadId;
        }
    }
}
