using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions.Extensions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace YS.Knife.Grpc.TestServer
{
    [GrpcService]
    public class GreeterService : Greeter.GreeterBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            HelloReply helloReply = new HelloReply
            {
                Message = $"Hello,{request.Name}"
            };
            return Task.FromResult(helloReply);
        }

        public override Task<ForecastListReply> GetForecast(EmptyRequest request, ServerCallContext context)
        {
            var rng = new Random();
            var data = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
                .ToArray();
            var reply = new ForecastListReply() { };
            reply.Forecast.AddRange(data.Select(p => new Forecast
            {
                Date = Timestamp.FromDateTime(p.Date.AsUtc()),
                TemperatureC = p.TemperatureC,
                TemperatureF = p.TemperatureF,
                Summary = p.Summary
            }));
            return Task.FromResult(reply);

        }
    }
}
