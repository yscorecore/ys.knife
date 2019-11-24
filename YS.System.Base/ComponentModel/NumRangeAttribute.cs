using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel
{
    /// <summary>
    /// 指定 数字的范围 。
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class NumRangeAttribute : Attribute
    {
        private double m_minvalue = 0;
        private double m_maxvalue = 100;
        private double m_mainStep = 10;
        private double m_viceStep = 1;
        /// <summary>
        /// 获取或设置最小值
        /// </summary>
        public double Minvalue
        {
            get { return m_minvalue; }
            set { m_minvalue = value; }
        }
        /// <summary>
        /// 获取或设置最大值
        /// </summary>
        public double Maxvalue
        {
            get { return m_maxvalue; }
            set { m_maxvalue = value; }
        }
        /// <summary>
        /// 获取或设置大步长
        /// </summary>
        public double LargeStep
        {
            get
            {
                return this.m_mainStep;
            }
            set
            {
                this.m_mainStep = value;
            }
        }
        /// <summary>
        /// 获取或设置小步长
        /// </summary>
        public double SmallStep
        {
            get
            {
                return this.m_viceStep;
            }
            set
            {
                this.m_viceStep = value;
            }
        }
        /// <summary>
        /// 初始化 <see cref="NumRangeAttribute"/> 的新实例。
        /// </summary>
        /// <param name="minvalue">最小值</param>
        /// <param name="maxvalue">最大值</param>
        public NumRangeAttribute(double minvalue, double maxvalue)
        {
            if (minvalue > maxvalue) { var temp = minvalue; minvalue = maxvalue; maxvalue = temp; }
            this.m_maxvalue = maxvalue;
            this.m_minvalue = minvalue;
            this.m_mainStep = GetLargeStep(minvalue, maxvalue);
            this.m_viceStep = GetSmallStep(this.m_mainStep);
        }
        /// <summary>
        /// 初始化 <see cref="NumRangeAttribute"/> 的新实例。
        /// </summary>
        /// <param name="minvalue">最小值</param>
        /// <param name="maxvalue">最大值</param>
        /// <param name="largeStep">大步长</param>
        /// <param name="smallStep">小步长</param>
        public NumRangeAttribute(double minvalue, double maxvalue, double largeStep, double smallStep)
        {
            if (minvalue > maxvalue) { var temp = minvalue; minvalue = maxvalue; maxvalue = temp; }
            this.m_maxvalue = maxvalue;
            this.m_minvalue = minvalue;
            largeStep = largeStep < 0 ? -largeStep : largeStep;
            smallStep = smallStep < 0 ? -smallStep : smallStep;
            if (largeStep == 0) largeStep = 1;
            if (smallStep == 0) smallStep = 1;
        }
        private double GetLargeStep(double min, double max)
        {
            double len = max - min;
            double num = len / 10.0;
            if (num <= 1) return 1;
            else return (int)num;
        }
        private double GetSmallStep(double largeSetp)
        {
            double num = largeSetp / 5.0;
            if (num <= 1) return 1;
            else return (int)num;

        }

    }
}
