using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace YS.Knife.HostedService.UnitTest
{
    [HostedService]
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
