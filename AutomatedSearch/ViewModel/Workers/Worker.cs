using System;
using System.Threading;
using AutomatedSearch.Workers.DataEntities;

namespace AutomatedSearch.ViewModel.Workers
{
    public class Worker : IWorker
    {
        private Thread _thread;
        private CancellationTokenSource _cancellationToken;

        private readonly object _arg;

        public Worker()
        {
            _cancellationToken = new CancellationTokenSource();
        }

        public void Start(Action operation)
        {
            if (_thread != null && _thread.IsAlive)
            {
                throw new Exception("The thread is already started!");
            }

            _thread = new Thread
            (
                delegate ()
                {
                    while (!_cancellationToken.IsCancellationRequested)
                    {
                        operation.Invoke();
                        return;
                    }

                    //Cleanup code
                }
            );

            _thread.Name = nameof(Worker);
            _thread.Start();
        }

        public void Start<T, R>(Func<T, R> operation, bool isBackground) where T : class
        {
            if (_thread != null && _thread.IsAlive)
            {
                throw new Exception("The thread is already started!");
            }

            _thread = new Thread
            (
                delegate ()
                {
                    while (!_cancellationToken.IsCancellationRequested)
                    {
                        operation.Invoke(_arg as T); // T or null
                    }

                    //Cleanup code
                }
            );

            _thread.Name = nameof(Worker);
            _thread.IsBackground = isBackground;

            if (isBackground)
            {
                _thread.Priority = ThreadPriority.BelowNormal;
            }

            _thread.Start();
        }

        public void Stop()
        {
            _cancellationToken.Cancel();
        }

        public void Dispose()
        {
            Stop();

            if (_thread.IsAlive)
            {
                _thread.Join(5000);
            }

            _thread = null;
            _cancellationToken.Dispose();
            _cancellationToken = null;
        }
    }
}
