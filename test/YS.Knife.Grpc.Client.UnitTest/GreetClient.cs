using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Grpc;
using YS.Knife.Grpc.Client.UnitTest;


public partial class Greeter
{
    [GrpcClient]
    public partial class GreeterClient : IGreeterService
    {

        public async Task<List<WeatherForecast>> GetForecast()
        {

            var reply = await GetForecastAsync(new EmptyRequest());
            return reply.Forecast.Select(p => new WeatherForecast
            {
                Date = p.Date.ToDateTime(),
                Summary = p.Summary,
                TemperatureC = p.TemperatureC,
                TemperatureF = p.TemperatureF
            }).ToList();
        }

        public async Task<string> SayHello(string message)
        {
            var reply = await SayHelloAsync(new HelloRequest { Name = message });
            return reply.Message;
        }
    }

}



