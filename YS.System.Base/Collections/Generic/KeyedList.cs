using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic
{
    public abstract class KeyedList<TItem,Tkey>:IList<TItem>
    {
        public KeyedList()
        {
        }
        public KeyedList(IEnumerable<TItem> items)
        {
            this.AddRange(items);
        }
        private List<Tkey> keys = new List<Tkey>();
        private Dictionary<Tkey,TItem> dics = new Dictionary<Tkey,TItem>();
        public int IndexOf(TItem item)
        {
            return this.keys.IndexOf(this.GetItemKey(item));
        }
        public void Insert(int index,TItem item)
        {
            Tkey k = this.GetItemKey(item);
            if(!this.dics.ContainsKey(k))
            {
                this.dics.Add(k,item);
                this.keys.Insert(index,k);
                
            }
            else
            {
                throw new ArgumentException("the insert item key has exist.");
            }
        }
        public void RemoveAt(int index)
        {
            if(index < 0 || index >= keys.Count)
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                Tkey key = this.keys[index];
                this.keys.RemoveAt(index);
                this.dics.Remove(key);
            }
        }
        public TItem this[int index]
        {
            get
            {
                Tkey key = this.keys[index];
                return dics[key];
            }
            set
            {
                if(index >= 0 && index < this.keys.Count)
                {
                    Tkey key = this.GetItemKey(value);
                    Tkey key2 = this.keys[index];
                    if(object.Equals(key,key2))//键值相等
                    {
                        dics[key] = value;
                    }
                    else
                    {
                        if(this.dics.ContainsKey(key))
                        {
                            throw new ArgumentException("the item key has exist.");
                        }
                        else
                        {
                            dics.Remove(key2);
                            dics.Add(key,value);
                            keys[index] = key;
                        }
                    }
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }
        public TItem this[Tkey key]
        {
            get {
                return dics[key];
            }
            set {
                if (value == null) throw new ArgumentNullException();
                
                AddOrCover(value);
            }
        }
        public void Add(TItem item)
        {
            Tkey k = this.GetItemKey(item);
            if(!this.dics.ContainsKey (k))
            {
                this.dics.Add(k,item);
                this.keys.Add(k);
            }
            else
            {
                throw new ArgumentException("the add item key has exist.");
            }
        }
        /// <summary>
        /// 添加或覆盖
        /// </summary>
        /// <param name="item"></param>
        public void AddOrCover(TItem item)
        {
            Tkey k = this.GetItemKey(item);
            if (!this.dics.ContainsKey(k))
            {
                this.dics.Add(k, item);
                this.keys.Add(k);
            }
            else
            {
                this.dics[k] = item;
            }
        }
        public void AddRange(IEnumerable<TItem> items)
        {
            foreach(var v in items) this.Add(v);
        }
        public void Clear()
        {
            this.dics.Clear();
            this.keys.Clear();
        }
        public void TrimExcess()
        {
            this.keys.TrimExcess();
        }
        public bool Contains(TItem item)
        {
            return this.dics.ContainsKey(this.GetItemKey(item));
        }
        public bool ContainsKey(Tkey key)
        {
            return this.dics.ContainsKey(key);
        }
        public void CopyTo(TItem[] array)
        {
            this.CopyTo(array,0);
        }
        public void CopyTo(TItem[] array,int arrayIndex)
        {
            if(array ==null)
            {
                throw new ArgumentNullException("array");
            }
            else if(arrayIndex < 0 ||arrayIndex>=array.Length)
            {
                throw new IndexOutOfRangeException();
            }
            else if(arrayIndex + this.Count > array.Length)
            {
                throw new ArgumentException("集合的容量不够");
            }else
            {
                foreach(Tkey k in this.keys)
                {
                    array[arrayIndex++] = dics[k];
                }
            }
        }
        public int Count
        {
            get { return this.keys.Count; }
        }
        public bool IsReadOnly
        {
            get { return false; }
        }
        public bool Remove(TItem item)
        {
            Tkey key = this.GetItemKey(item);
            int index=this.keys .IndexOf(key);
            if(index >= 0)
            {
                this.keys.RemoveAt(index);
                this.dics.Remove(key);
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool RemoveByKey(Tkey key)
        {
            int index = this.keys.IndexOf(key);
            if (index >= 0)
            {
                this.keys.RemoveAt(index);
                this.dics.Remove(key);
                return true;
            }
            else
            {
                return false;
            }
            
        }
        public IEnumerator<TItem> GetEnumerator()
        {
            foreach(var key in this.keys)
            {
                yield return dics[key];
            }
        }
        public IEnumerator<Tkey> GetKeyEnumerator()
        {
            return this.keys.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public abstract Tkey GetItemKey(TItem item);


    }
}
