using System;
using System.Threading;
using Microsoft.Extensions.Options;
using YS.Knife;
using YS.Knife.;

namespace App
{
    [HostedClass]
    public class Service1 : TimerTickHostedService
    {
        public Service1(IOptionsMonitor<AppOptions> options)
        {
            this.Options = options;
        }
        public IOptionsMonitor<AppOptions> Options { get; }
        protected override TimeSpan Interval => TimeSpan.FromSeconds(1);

        protected override void OnException(Exception exception)
        {
        }

        protected override void OnTick(CancellationToken state)
        {
            Console.WriteLine($"{DateTimeOffset.UtcNow} {Options.CurrentValue.GetHashCode():X} Str:{Options.CurrentValue.StrProp}\t\tInt:{Options.CurrentValue.IntProp}");
        }
    }
}
