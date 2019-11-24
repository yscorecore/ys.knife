using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace System.Collections.Generic
{
    [Serializable]
    public class LimitedHashSet<T>:ICollection<T>,IEnumerable<T>,IEnumerable
    {
        HashSet<T> set = new HashSet<T>();
        Queue<T> queue = new Queue<T>();


        public void Add(T item)
        {
            
        }

        public void Clear()
        {
            set.Clear();
            queue.Clear();
           // throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            return set.Contains(item);
        }

        public void CopyTo(T[] array,int arrayIndex)
        {
             set.CopyTo(array,arrayIndex);
        }

        public int Count
        {
            get { return set.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
