using System;

namespace AutomatedSearch.Workers.DataEntities
{
    public interface IWorker : IDisposable
    {
        public void Start(Action operation);
        public void Start<T, R>(Func<T, R> operation, bool isBackground) where T : class;
        public void Stop();
    }
}
