using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class DoubleEx
    {
        /// <summary>
        /// 判断给定的数是否属于两个数字之间(不考虑两个数字的顺序)
        /// </summary>
        /// <param name="val">给定的数字</param>
        /// <param name="num1">数字1</param>
        /// <param name="num2">数字2</param>
        /// <returns></returns>
        public static bool Between(this double val, double num1,double num2)
        {
            return (val - num1) * (val - num2) < 0;
        }
        public static bool BetweenEx(this double val,double num1,double num2)
        {
            return (val - num1) * (val - num2)<=0;
        }
    }

}
