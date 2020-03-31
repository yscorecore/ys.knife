using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace YS.Knife.HostedService
{
    public abstract class LoopHostedService : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                try
                {
                    this.OnLoop(stoppingToken);
                }
#pragma warning disable CA1031 // 不捕获常规异常类型
                catch (Exception ex)
#pragma warning restore CA1031 // 不捕获常规异常类型
                {
                    this.OnException(ex);
                }
            }

            return stoppingToken.IsCancellationRequested
                        ? Task.FromCanceled(stoppingToken)
                        : Task.CompletedTask;
        }

        protected abstract void OnLoop(CancellationToken stoppingToken);
        protected abstract void OnException(Exception exception);
    }
}
