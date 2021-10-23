using System;
using BenchmarkDotNet.Running;

namespace Mapper.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var user = new { Age=10};
            user.MyMap<dynamic, UserDto>();
           // _ = BenchmarkRunner.Run<MapperTest>();
        }
    }
}
