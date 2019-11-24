using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示自增序列
    /// </summary>
    [Serializable]
    public class Sequence
    {
        #region 静态 （隐式转换）
        public static implicit operator int(Sequence seq)
        {
            return seq.current;
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化<see cref="Sequence"/>的新实例
        /// </summary>
        public Sequence()
        {

        }
        /// <summary>
        /// 初始化<see cref="Sequence"/>的新实例
        /// </summary>
        /// <param name="startValue">开始值</param>
        /// <param name="step">步长</param>
        public Sequence(int startValue, int step = 1)
        {
            this.current = startValue;
            this.step = step;
        }
        #endregion

        #region 属性
        private int current;

        /// <summary>
        /// 获取当前的值
        /// </summary>
        public int Current
        {
            get { return current; }
        }
        private int step = 1;
        /// <summary>
        /// 获取步长
        /// </summary>
        public int Step
        {
            get { return step; }
        }
        #endregion
        /// <summary>
        /// 重置序列
        /// </summary>
        /// <param name="startValue">序列的起始值</param>
        /// <param name="step">序列的步长</param>
        public void Reset(int startValue, int step = 1)
        {
            this.current = startValue;
            this.step = step;
        }
        /// <summary>
        /// 从序列中获取一个值，并重置当前的值。
        /// </summary>
        /// <returns></returns>
        public virtual int GetValue()
        {
            lock (this)
            {
                return current += step;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}", this.current);
        }
    }

    [Serializable]
    public class LoopSequence
    {
        #region 静态 （隐式转换）
        public static implicit operator int(LoopSequence seq)
        {
            return seq.current;
        }
        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化<see cref="Sequence"/>的新实例
        /// </summary>
        /// <param name="startValue">开始值</param>
        /// <param name="step">步长</param>
        public LoopSequence(int startValue, int endValue, int step = 1)
        {
            if (endValue < startValue)
            {
                throw new ArgumentException("endValue 应该大于startValue");
            }
            this.startValue = startValue;
            this.endValue = endValue;
            this.current = startValue;
            this.step = step;
        }
        #endregion

        #region 属性
        private int current;

        /// <summary>
        /// 获取当前的值
        /// </summary>
        public int Current
        {
            get { return current; }
        }
        private int step = 1;
        /// <summary>
        /// 获取步长
        /// </summary>
        public int Step
        {
            get { return step; }
        }
        private int startValue;
        public int StartValue
        {
            get { return startValue; }
        }
        private int endValue;
        public int EndValue
        {
            get { return endValue; }
        }
        #endregion
        /// <summary>
        /// 重置序列
        /// </summary>
        /// <param name="startValue">序列的起始值</param>
        /// <param name="step">序列的步长</param>
        public void Reset()
        {
            this.current = startValue;
        }
        /// <summary>
        /// 从序列中获取一个值，并重置当前的值。
        /// </summary>
        /// <returns></returns>
        public int GetValue()
        {
            lock (this)
            {
                current += step;
                if (current >= endValue)
                {
                    current = current - endValue + startValue;
                }
                return current;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}", this.current);
        }
    }
}
