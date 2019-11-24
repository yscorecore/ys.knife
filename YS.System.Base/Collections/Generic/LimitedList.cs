namespace System.Collections.Generic
{
      using System;
      using System.Diagnostics;

      /// <summary>
      /// 表示有容量限制的强类型列表
      /// </summary>
      /// <typeparam name="T">列表类型</typeparam>
      [Serializable()]
      [DebuggerDisplay("Count/LimitedCount = {Count}/{LimitedCount}")]
      public class LimitedList<T> : IList<T>, IEnumerable<T>, ICollection, IEnumerable
      {
            List<T> lst = new List<T>();
            private  int m_limitedCount;
            #region 构造函数
            public LimitedList():this(100)
            {

            }
            public LimitedList(int limitedCount)
                  : this(limitedCount, new T[0])
            {

            }
            public LimitedList(int limitedCount, IEnumerable<T> items)
            {
                  this.LimitedCount = limitedCount;
                  this.AddRange(items);
            }
            #endregion
            /// <summary>
            /// 获取或设置列表的限制数量
            /// </summary>
            /// <exception cref="ArgumentOutOfRangeException">设置的数量小于0</exception>
            public int LimitedCount
            {
                  get { return m_limitedCount; }
                  set
                  {
                        if (m_limitedCount < 0)
                        {
                              throw new ArgumentOutOfRangeException("value");
                        }
                        m_limitedCount = value;
                        this.CheckCount();
                  }
            }

            public int IndexOf(T item)
            {
                  return lst.IndexOf(item);
            }

            public void Insert(int index, T item)
            {
                  lst.Insert(index, item);
                  this.CheckCount();
            }

            public void RemoveAt(int index)
            {
                  lst.RemoveAt(index);
            }

            public T this[int index]
            {
                  get
                  {
                        return lst[index];
                  }
                  set
                  {
                        lst[index] = value;
                  }
            }

            public void Add(T item)
            {
                  lst.Add(item);
                  this.CheckCount();
            }

            public void AddRange(IEnumerable<T> items)
            {
                  this.lst.AddRange(items);
                  this.CheckCount();
            }

            public void Clear()
            {
                  lst.Clear();
            }

            public bool Contains(T item)
            {
                  return lst.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                  lst.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                  get { return lst.Count; }
            }

            public bool IsReadOnly
            {
                  get { return true; }
            }

            public bool Remove(T item)
            {
                  return lst.Remove(item);
            }

            public IEnumerator<T> GetEnumerator()
            {
                  return lst.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                  return lst.GetEnumerator();

            }
            private void CheckCount()
            {
                  if (lst.Count > this.m_limitedCount)
                  {
                        lst.RemoveRange(0, this.m_limitedCount - this.Count);
                  }
            }

            public void CopyTo(Array array, int index)
            {
                  (this.lst as ICollection).CopyTo(array, index);
            }

            public bool IsSynchronized
            {
                  get { return true; }
            }

            public object SyncRoot
            {
                  get { return this; }
            }
      }
}
