using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    [System.Diagnostics.DebuggerDisplay("Index={index} ,Value={_value}")]
    [Serializable]
    public struct IndexData<T>
    {
        public IndexData(int index, T value)
        {
            this.index = index;
            this._value = value;
        }
        private int index;

        public int Index
        {
            get { return index; }
            set { index = value; }
        }
        private T _value;

        public T Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override string ToString()
        {
            return string.Format("[{0}]={1}", index, _value);
        }
    }
}
