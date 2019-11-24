using System;
using System.Collections.Generic;
using System.Text;

namespace System.Mathematics.Fitting
{
    /// <summary>
    /// 提供拟合结果的信息
    /// </summary>
    public abstract  class FittingResultInfoBase
    {
        private double m_r = 0f;
         /// <summary>
        /// 初始化 <see cref="FittingResultInfoBase"/> 的新实例。
        /// </summary>
        /// <param name="r">相关系数</param>
        internal FittingResultInfoBase (double r) {
            this.m_r = r;
        }
        /// <summary>
        /// 获取拟合结果的相关系数
        /// </summary>
        public double R {
            get { return m_r; }
        }
        ///// <summary>
        ///// 获取拟合公式的字串。
        ///// </summary>
        //public abstract MathFormula Formula{get;}
    }
}
