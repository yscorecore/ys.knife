using System;
using System.Collections.Generic;
using System.Text;

namespace System.Mathematics.Fitting
{
    /// <summary>
    /// 提供常用的利用最小二乘法拟合的函数
    /// </summary>
    public class LeastSquareFitting
    {
        /// <summary>
        /// 利用最小二乘法对输入数据进行线性拟合。
        /// </summary>
        /// <param name="data">拟合的数据</param>
        /// <returns>线性拟合的结果</returns>
        public static LinearFittingResultInfo LinearFitting (Descartes2DCoor[] datas) {
            if (datas == null) throw new ArgumentNullException("datas");
            if (datas.Length < 2) throw new ArgumentException("拟合的数据点个数不够。");
            double a = MathEx.AverageSubProductSumXY(datas) / MathEx.AverageSubSquareSumX(datas);
            double b = MathEx.AverageY(datas) - a * MathEx.AverageX(datas);
            double r = MathEx.AverageSubProductSumXY(datas) /
                (Math.Sqrt(MathEx.AverageSubSquareSumX(datas) * MathEx.AverageSubSquareSumY(datas)));
            return new LinearFittingResultInfo(a, b, r);
       
        }
    }
}
