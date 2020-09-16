using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace YS.Knife.HostedService
{
    public abstract class TimerTickHostedService : IHostedService, IDisposable
    {
        private CancellationTokenSource _stoppingCts =
                                                   new CancellationTokenSource();
        private Timer _timer;

        protected abstract TimeSpan Interval { get; }

        protected virtual TimeSpan WaitTimeBeforeFirstInterval
        {
            get { return TimeSpan.FromSeconds(1); }
        }
        //是否将时间同步到整数秒
        protected virtual bool IntegerSecond
        {
            get { return true; }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            TimeSpan waitTime = IntegerSecond ? WaitTimeBeforeFirstInterval + TimeSpan.FromMilliseconds(1000 - DateTimeOffset.Now.Millisecond) : WaitTimeBeforeFirstInterval;
            if (_timer == null)
            {
                _timer = new Timer(this.TryTick, _stoppingCts.Token, waitTime, Interval);
            }
            else
            {
                _timer.Change(waitTime, Interval);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
            _stoppingCts.Cancel();
            return Task.CompletedTask;
        }
        private void TryTick(object state)
        {
            var cancelToken = (CancellationToken)state;
            if (cancelToken.IsCancellationRequested) return;
            try
            {
                this.OnTick(cancelToken);
            }
#pragma warning disable CA1031 // 不捕获常规异常类型
            catch (Exception ex)
#pragma warning restore CA1031 // 不捕获常规异常类型
            {
                this.OnException(ex);
            }
        }
        protected abstract void OnTick(CancellationToken state);
        protected abstract void OnException(Exception exception);

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this._timer.Dispose();
                    this._timer = null;
                    this._stoppingCts.Dispose();
                    this._stoppingCts = null;
                }
                disposedValue = true;
            }
        }



        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
