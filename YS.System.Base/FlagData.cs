using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
   /// <summary>
   /// 表示一个标志位和数据组成的混合结构体
   /// </summary>
   /// 
   /// <typeparam name="T"></typeparam>
   /// <example>
   /// var v=this.CanStart();
   /// if(v) this.Start();
   /// else MessageBox.Show(v.Item);
   /// </example>
    [System.Diagnostics.DebuggerDisplay("Flag={flag} ,Item={item}")]
    [Serializable]
    public struct FlagData<T>//BooleanEx
    {
        public static implicit operator bool(FlagData<T> b)
        {
            return  b.Flag;
        }
        public static implicit operator FlagData<T>(bool b)
        {
            return new FlagData<T>(b);
        }
        public FlagData(bool flag)
            : this(flag, default(T))
        {
        }
        public FlagData(bool flag, T item)
        {
            this.flag = flag;
            this.item = item;
        }
        private bool flag;

        public bool Flag
        {
            get { return flag; }
            set { flag = value; }
        }
        private T item;

        public T Item
        {
            get { return item; }
            set { item = value; }
        }

        
    }
}
