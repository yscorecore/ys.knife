using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示样品提供者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SampleProvider<T>
    {
        private List<T> lst = new List<T>();
        private int m_index = 0;
        public SampleProvider()
        {

        }
        public SampleProvider(IEnumerable<T> samples)
        {
            this.lst.AddRange(samples);
        }
        public void AddSample(T sample)
        {
            lock (lst)
            {
                this.lst.Add(sample);
            }
        }
        public void AddSamples(IEnumerable<T> samples)
        {
            lock (lst)
            {
                this.lst.AddRange(samples);
            }
        }
        /// <summary>
        /// 重置索引
        /// </summary>
        public void ResetIndex()
        {
            lock (lst)
            {
                m_index = 0;
            }
        }
        public T GetSampleValue()
        {
            lock (lst)
            {
                if (lst.Count > 0)
                {
                    if (m_index >= lst.Count) m_index = 0;
                    return lst[m_index++];
                }
                else
                {
                    return default(T);
                }
            }
        }
        public T[] GetSampleValues(int count)
        {
            lock (lst)
            {
                if (count < 0) throw new ArgumentOutOfRangeException("count");
                T[] arr = new T[count];
                if (lst.Count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (m_index >= lst.Count) m_index = 0;
                        arr[i] = lst[m_index++];
                    }
                }
                return arr;
            }
        }

    }
}
