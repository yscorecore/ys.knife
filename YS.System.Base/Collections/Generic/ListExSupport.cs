using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic
{
      public class ListItemChangeEventArgs<T> : EventArgs
      {
            public ListItemChangeEventArgs(T originalItem, T currentItem)
            {
                  this.OriginalItem = originalItem;
            }
            public T OriginalItem { get; private set; }
            public T CurrentItem { get; private set; }
      }
      public enum ListExChangeAction
      {
            Add,
            Insert,
            Remove,
            Clear,
            AddRange,
            SetItemByIndex,
            Reset,
            ItemPropertyChanged,
      }

      public class ListExChangingEventArgs<T> : EventArgs
      {
            public ListExChangingEventArgs(ListExChangeAction changeAction, object actionObject)
                  : this(changeAction, actionObject, false)
            {

            }
            public ListExChangingEventArgs(ListExChangeAction changeAction, object actionObject, bool cancel)
            {
                  this.ChangeAction = changeAction;
                  this.ActionObject = actionObject;
                  this.Cancel = cancel;
            }
            public bool Cancel { get; set; }
            public ListExChangeAction ChangeAction { get; private set; }
            public object ActionObject { get; private set; }

      }
      public class ListExChangedEventArgs<T> : EventArgs
      {
            public ListExChangedEventArgs(ListExChangeAction changeAction, object actionObject)
            {
                  this.ChangeAction = changeAction;
                  this.ActionObject = actionObject;
            }
            public ListExChangeAction ChangeAction { get; private set; }
            public object ActionObject { get; private set; }
      }
}
