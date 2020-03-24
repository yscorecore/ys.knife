using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SwaggerDemo.Core;

namespace SwaggerDemo.Api
{
    public class CalcController : YS.Knife.Api.ApiBase<ICalcService>, ICalcService
    {
        /// <summary>
        /// 加法运算
        /// </summary>
        /// <param name="a">加数</param>
        /// <param name="b">被加数</param>
        /// <returns></returns>
        [HttpGet]
        [Route("add")]
        public Task<int> Add([FromQuery]int a, [FromQuery]int b)
        {
            return this.Delegater.Add(a, b);
        }
        /// <summary>
        /// 减法运算
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("sub")]
        public Task<int> Sub([FromQuery]int a, [FromQuery]int b)
        {
            return this.Delegater.Sub(a, b);
        }
    }
}
