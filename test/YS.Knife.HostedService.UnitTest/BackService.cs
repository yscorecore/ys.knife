using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YS.Knife.HostedService.UnitTest
{
    [HostedClass]
    public class BackService : BackgroundService
    {
        public static bool Triggered = false;
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Triggered = true;
            return Task.CompletedTask;
        }
    }
}
