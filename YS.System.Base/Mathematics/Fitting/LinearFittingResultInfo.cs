using System;
using System.Collections.Generic;
using System.Text;

namespace System.Mathematics.Fitting
{
    /// <summary>
    /// 提供线性拟合结果的信息
    /// </summary>
    public class LinearFittingResultInfo : FittingResultInfoBase
    {
        private readonly double m_a;
        private readonly double m_b;

        /// <summary>
        /// 初始化 <see cref="LinearFittingResultInfo"/> 的新实例。
        /// </summary>
        /// <param name="a">系数a,斜率</param>
        /// <param name="b">系数b,截距</param>
        /// <param name="r">相关系数</param>
        internal LinearFittingResultInfo (double a, double b, double r)
            : base(r) {
            this.m_a = a;
            this.m_b = b;
        }

        #region 属性
        /// <summary>
        /// 获取系数 a ,斜率
        /// </summary>
        public double A {
            get { return m_a; }
        }
        /// <summary>
        /// 获取系数 b，截距
        /// </summary>
        public double B {
            get { return m_b; }
        }
        ///// <summary>
        ///// 获取拟合公式的字串。
        ///// </summary>
        ///// <value></value>
        //public override MathFormula Formula {
        //    get {
        //         return new  MathFormula("Y = aX + b");
        //    }
        //}
        #endregion
    }
}
