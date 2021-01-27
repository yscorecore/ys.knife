using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Grpc.Client.UnitTest
{
    public interface IGreeterService
    {
        Task<string> SayHello(string message);



        Task<List<WeatherForecast>> GetForecast();

    }

    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF { get; set; }

        public string Summary { get; set; }
    }
}
