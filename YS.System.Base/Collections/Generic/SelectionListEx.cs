using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic
{
      public class SelectionListEx<T> : ListEx<T>
      {
            private ListEx<T> m_selectionItems = new ListEx<T>();
            #region 事件
            public event EventHandler<ListExChangingEventArgs<T>> SelectionItemsChanging
            {
                  add
                  {
                        this.m_selectionItems.ListChanging += value;
                  }
                  remove
                  {
                        this.m_selectionItems.ListChanging -= value;
                  }
            }

            public event EventHandler<ListExChangedEventArgs<T>> SelectionItemsChanged
            {
                  add
                  {
                        this.m_selectionItems.ListChanged += value;
                  }
                  remove
                  {
                        this.m_selectionItems.ListChanged -= value;
                  }
            }
            #endregion
            public ListEx<T> SelectionItems
            {
                  get { return m_selectionItems; }
            }
            public void ResetSelections(IEnumerable<T> newSelections)
            {
                  this.m_selectionItems.Reset(newSelections);
            }
            public void ResetSelections(T[] newSelections)
            {
                  this.ResetSelections(newSelections);
            }
            public void ClearSelections()
            {
                  this.m_selectionItems.Clear();
            }
            public void AppendSelectionItem(T item)
            {
                  this.m_selectionItems.Add(item);
            }
            public void RemoveSelectionItem(T item)
            {
                  this.m_selectionItems.Remove(item);
            }
            public void ReverseSelectionItem(T item)
            {
                  if (this.m_selectionItems.Contains(item))
                  {
                        this.m_selectionItems.Remove(item);
                  }
                  else
                  {
                        this.m_selectionItems.Add(item);
                  }
            }
      }
}
