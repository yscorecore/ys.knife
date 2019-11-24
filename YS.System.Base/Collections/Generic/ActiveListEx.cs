using System;
using System.Collections.Generic;
using System.Text;


namespace System.Collections.Generic
{
      public class ActiveListEx<T> : ListEx<T>
      {
            public event EventHandler<ListItemChangeEventArgs<T>> ActiveItemChanging;
            public event EventHandler<ListItemChangeEventArgs<T>> ActiveItemChanged;
            private T m_activeITem;
            public T ActiveItem
            {
                  get
                  {
                        return m_activeITem;
                  }
                  set
                  {
                        if (!this.ItemEquals(this.m_activeITem,value ))
                        {
                              ListItemChangeEventArgs<T> arg = new ListItemChangeEventArgs<T>(m_activeITem, value);
                              this.OnActiveItemChanging(arg);
                              this.m_activeITem = value;
                              this.OnActiveItemChanged(arg);
                        }
                  }
            }
          
            protected virtual void OnActiveItemChanging(ListItemChangeEventArgs<T> e)
            {
                  if (!this.IsSuspended)
                  {
                        if (this.ActiveItemChanging != null)
                        {
                              this.ActiveItemChanging(this, e);
                        }
                  }
            }
            protected virtual void OnActiveItemChanged(ListItemChangeEventArgs<T> e)
            {
                  if (!this.IsSuspended)
                  {
                        if (this.ActiveItemChanged != null)
                        {
                              this.ActiveItemChanged(this, e);
                        }
                  }
            }
      }
}
