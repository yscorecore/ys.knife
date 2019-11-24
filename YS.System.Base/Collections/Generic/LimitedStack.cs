

namespace System.Collections.Generic
{
      using System;
      using System.Collections.Generic;
      using System.Text;

      public class LimitedStack<T> :IEnumerable<T>, ICollection, IEnumerable
      {
            private const int DEFAULTMAXCOUNT = 100;
            Stack<T> stack;
            private readonly int m_limitedCount;

          

            #region  构造函数
            public LimitedStack() :this(DEFAULTMAXCOUNT)
            {

            }
            public LimitedStack(int limitedCount) :this(limitedCount,new T[0])
            {
                 
            }
            public LimitedStack(int limitedCount,IEnumerable<T> items)
            {
                  if (limitedCount <= 0)
                  {
                        throw new ArgumentOutOfRangeException("limitedCount", "最大数量必须为正整数");
                  }
                  this.m_limitedCount = limitedCount;
                  stack = new Stack<T>(limitedCount);
                  if (items != null)
                  {
                        foreach (T t in items)
                        {
                              this.Push(t);  
                        }
                  }
            }
            #endregion

            public int Count
            {
                  get 
                  {
                        return stack.Count;
                  }
            }
            public int LimitedCount
            {
                  get { return m_limitedCount; }
            }

            public void Push(T item)
            {
                  this.stack.Push(item);
                  this.CheckCount();
            }
            public T Pop()
            {
                  return stack.Pop();
            }
            public T Peek()
            {
                  return stack.Peek();
            }
            public void Clear()
            {
                  this.stack.Clear();
            }
            public bool Contains(T item)
            {
                  return this.stack.Contains(item);
            }
            public void CopyTo(T[] array, int arrayIndex)
            {
                  this.stack.CopyTo(array, arrayIndex);
            }
            public T[] ToArray()
            {
                  return this.stack.ToArray();
            } 
            private void CheckCount()
            {
                  while (stack.Count > this.m_limitedCount)
                  {
                        stack.Pop();
                  }
            }

            #region 显示接口

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                  return this.stack.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                  return this.stack.GetEnumerator();
            }

            void ICollection.CopyTo(Array array, int index)
            {
                  this.stack.CopyTo(array as T[], index);
            }

            bool ICollection.IsSynchronized
            {
                  get { return true; }
            }

            object ICollection.SyncRoot
            {
                  get { return this; }
            }

            #endregion
      }
}
