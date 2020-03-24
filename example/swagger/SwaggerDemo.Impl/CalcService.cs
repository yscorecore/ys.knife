using System;
using System.Threading.Tasks;
using SwaggerDemo.Core;
using YS.Knife;

namespace SwaggerDemo.Impl
{
    [ServiceClass]
    public class CalcService : ICalcService
    {
        public Task<int> Add(int a, int b)
        {
            return Task.FromResult(a + b);
        }

        public Task<int> Sub(int a, int b)
        {
            return Task.FromResult(a - b);
        }
    }
}
