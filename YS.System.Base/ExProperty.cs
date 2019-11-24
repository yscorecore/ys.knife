using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示拓展属性
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class ExProperty<TSource, TValue> : IEnumerable<KeyValuePair<TSource, TValue>>
        where TSource : new()
    {
        #region 构造函数
        public ExProperty()
            : this(default(TValue))
        {

        }
        public ExProperty(TValue defaultValue)
        {
            this.defaultValue = defaultValue;
        }
        #endregion
        private TValue defaultValue;

        public virtual TValue DefaultValue
        {
            get { return defaultValue; }
        }
        Dictionary<TSource, TValue> datas = new Dictionary<TSource, TValue>();

        #region IDictionary<TSource,TValue> 成员

        public void Add(TSource key, TValue value)
        {
            this.datas.Add(key, value);
        }

        public bool ContainsKey(TSource key)
        {
            return this.datas.ContainsKey(key);
        }

        public ICollection<TSource> Keys
        {
            get { return this.datas.Keys; }
        }

        public bool Remove(TSource key)
        {
            return this.datas.Remove(key);
        }

        public ICollection<TValue> Values
        {
            get { return this.Values; }
        }

        public TValue this[TSource key]
        {
            get
            {
                if (this.datas.ContainsKey(key))
                {
                    return this.datas[key];

                }
                else
                {
                    return this.defaultValue;
                }
            }
            set
            {
                this.datas[key] = value;
            }
        }



        public void Clear()
        {
            this.datas.Clear();
        }


        public int Count
        {
            get { return this.datas.Count; }
        }





        #endregion


        public IEnumerator<KeyValuePair<TSource, TValue>> GetEnumerator()
        {
            return this.datas.GetEnumerator();
        }

        Collections.IEnumerator Collections.IEnumerable.GetEnumerator()
        {
            return this.datas.GetEnumerator();
        }

    }
}
