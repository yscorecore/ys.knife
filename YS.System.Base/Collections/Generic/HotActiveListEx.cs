using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic
{
      public class HotActiveListEx<T> : ActiveListEx<T>
      {
            public event EventHandler<ListItemChangeEventArgs<T>> HotItemChanging;
            public event EventHandler<ListItemChangeEventArgs<T>> HotItemChanged;
            private T m_hotITem;
            public T HotItem
            {
                  get
                  {
                        return m_hotITem;
                  }
                  set
                  {
                        if (!this.ItemEquals(this.m_hotITem,value))
                        {
                              ListItemChangeEventArgs<T> arg = new ListItemChangeEventArgs<T>(m_hotITem, value);
                              this.OnHotItemChanging(arg);
                              this.m_hotITem = value;
                              this.OnHotItemChanged(arg);
                        }
                  }
            }
            protected virtual void OnHotItemChanging(ListItemChangeEventArgs<T> e)
            {
                  if (!this.IsSuspended)
                  {
                        if (this.HotItemChanging != null)
                        {
                              this.HotItemChanging(this, e);
                        }
                  }
            }
            protected virtual void OnHotItemChanged(ListItemChangeEventArgs<T> e)
            {
                  if (!this.IsSuspended)
                  {
                        if (this.HotItemChanged != null)
                        {
                              this.HotItemChanged(this, e);
                        }
                  }
            }
      }
}
