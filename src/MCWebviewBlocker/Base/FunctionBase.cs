using System;
using System.Threading;

namespace MCWebviewBlocker
{
    public delegate void FunctionHandler();

    public class FunctionBase
    {
        private Thread _thread;

        private readonly CancellationTokenSource _ctx;
        private readonly uint _sleep;

        public event FunctionHandler Function;

        public FunctionBase(uint WaitforMSec = 0)
        {
            _ctx = new CancellationTokenSource();
            _sleep = WaitforMSec;

            return;
        }

        ~FunctionBase()
        {
            try
            {
                _ctx.Cancel();
            }
            catch { }

            try
            {
                if (_thread?.ThreadState == ThreadState.Running)
                {
                    _thread?.Abort();
                }
            }
            catch { }

            return;
        }

        public void Start()
        {
            _thread = new Thread(x =>
            {
                while (false == _ctx.IsCancellationRequested)
                {
                    Function?.Invoke();

                    Thread.Sleep(TimeSpan.FromMilliseconds(_sleep));
                }

                return;
            });

            _thread.Start();

            return;
        }

        public void Stop()
        {
            _ctx.Cancel();

            return;
        }

        public void Join()
        {
            _thread?.Join();

            return;
        }
    }
}
