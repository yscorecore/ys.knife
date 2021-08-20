using System;
using BenchmarkDotNet.Running;

namespace Mapper.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            _ = BenchmarkRunner.Run<MapperTest>();
        }
    }
}
