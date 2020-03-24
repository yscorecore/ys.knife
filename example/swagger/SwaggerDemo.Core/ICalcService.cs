using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SwaggerDemo.Core
{
    public interface ICalcService
    {
        Task<int> Add(int a, int b);

        Task<int> Sub(int a, int b);
    }
}
