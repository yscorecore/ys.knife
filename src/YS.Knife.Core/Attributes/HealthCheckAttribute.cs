using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace YS.Knife
{
    public class HealthCheckAttribute : KnifeAttribute
    {
        public HealthCheckAttribute() : base(typeof(IHealthCheck))
        {
        }
        public string Name { get; set; }
        public HealthStatus? FailureStatus { get; set; }
        public string[] Tags { get; set; }
        public TimeSpan? TimeOut { get; set; }

        public override void RegisterService(IServiceCollection services, IRegisterContext context, Type declareType)
        {
            services.AddHealthChecks();
            string healthCheckName = this.Name == null ? declareType?.FullName : this.Name;
            services.Configure<HealthCheckServiceOptions>(option =>
            {
                this.RemoveTheSameName(option.Registrations, healthCheckName);
                this.AddHealthCheck(option.Registrations, declareType, healthCheckName);

            });
        }
        private void RemoveTheSameName(ICollection<HealthCheckRegistration> healthChecks, string healthCheckName)
        {
            var shouldRemoved = healthChecks.Where(p => p.Name == healthCheckName).ToArray();
            Array.ForEach(shouldRemoved, (t) => healthChecks.Remove(t));
        }
        private void AddHealthCheck(ICollection<HealthCheckRegistration> healthChecks, Type healthCheckType, string healthCheckName)
        {
            var registration = new HealthCheckRegistration(healthCheckName, s => (IHealthCheck)ActivatorUtilities.GetServiceOrCreateInstance(s, healthCheckType), FailureStatus, Tags, TimeOut);
            healthChecks.Add(registration);
        }

    }
}
