using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic
{
      public class ListEx<T> : IList<T>
      {
            #region 挂起事件
            private int _suspendCount;
            public void SuspendEvents()
            {
                  _suspendCount++;
            }
            public void ResumeEvents()
            {
                  _suspendCount = 0;
            }
            public bool IsSuspended
            {
                  get { return (_suspendCount > 0); }
            }
            #endregion

            #region 1
            List<T> lst = new List<T>();
            public event EventHandler<ListExChangingEventArgs<T>> ListChanging;
            public event EventHandler<ListExChangedEventArgs<T>> ListChanged;
            public virtual void OnListChanging(ListExChangingEventArgs<T> e)
            {
                  if (!this.IsSuspended && this.ListChanging != null)
                  {
                        this.ListChanging(this, e);
                  }
            }
            public virtual void OnListChanged(ListExChangedEventArgs<T> e)
            {
                  if (!this.IsSuspended && this.ListChanged != null)
                  {
                        this.ListChanged(this, e);
                  }
            }



            public int IndexOf(T item)
            {
                  return lst.IndexOf(item);
            }

            public void Insert(int index, T item)
            {
                  ListExChangingEventArgs<T> before = new ListExChangingEventArgs<T>(ListExChangeAction.Insert, item);
                  this.OnListChanging(before);
                  if (!before.Cancel)
                  {
                        this.lst.Insert(index, item);
                        ListExChangedEventArgs<T> after = new ListExChangedEventArgs<T>(ListExChangeAction.Insert, item);
                        this.OnListChanged(after);
                  }
            }

            public void Reset(IEnumerable<T> newItems)
            {
                  ListExChangingEventArgs<T> before = new ListExChangingEventArgs<T>(ListExChangeAction.Reset, newItems);
                  this.OnListChanging(before);
                  if (!before.Cancel)
                  {
                        this.lst.Clear();
                        this.lst.AddRange(newItems);
                        ListExChangedEventArgs<T> after = new ListExChangedEventArgs<T>(ListExChangeAction.Reset, newItems);
                        this.OnListChanged(after);
                  }
            }
            public void Reset(params T[] newItems)
            {
                  this.Reset(newItems as IEnumerable<T>);
            }
            public void RemoveAt(int index)
            {
                  T item = this[index];
                  ListExChangingEventArgs<T> before = new ListExChangingEventArgs<T>(ListExChangeAction.Remove, item);
                  this.OnListChanging(before);
                  if (!before.Cancel)
                  {
                        this.lst.Remove(item);
                        ListExChangedEventArgs<T> after = new ListExChangedEventArgs<T>(ListExChangeAction.Remove, item);
                        this.OnListChanged(after);
                  }
            }

            public T this[int index]
            {
                  get
                  {
                        return this.lst[index];
                  }
                  set
                  {
                        if (!this.ItemEquals(this.lst[index], value))
                        {
                              ListExChangingEventArgs<T> before = new ListExChangingEventArgs<T>(ListExChangeAction.SetItemByIndex, new object[] { this.lst[index], value });
                              this.OnListChanging(before);
                              if (!before.Cancel)
                              {
                                    this.lst[index] = value;
                                    ListExChangedEventArgs<T> after = new ListExChangedEventArgs<T>(ListExChangeAction.SetItemByIndex, new object[] { this.lst[index], value });
                                    this.OnListChanged(after);
                              }
                        }
                  }
            }

            protected bool ItemEquals(T item1, T item2)
            {
                  return object.ReferenceEquals(item1, item2);
            }
            public void Add(T item)
            {
                  ListExChangingEventArgs<T> before = new ListExChangingEventArgs<T>(ListExChangeAction.Add, item);
                  this.OnListChanging(before);
                  if (!before.Cancel)
                  {
                        this.lst.Add(item);
                        ListExChangedEventArgs<T> after = new ListExChangedEventArgs<T>(ListExChangeAction.Add, item);
                        this.OnListChanged(after);
                  }
            }
            public void AddRange(IEnumerable<T> items)
            {
                  ListExChangingEventArgs<T> before = new ListExChangingEventArgs<T>(ListExChangeAction.AddRange, items);
                  this.OnListChanging(before);
                  if (!before.Cancel)
                  {
                        this.lst.AddRange(items);
                        ListExChangedEventArgs<T> after = new ListExChangedEventArgs<T>(ListExChangeAction.AddRange, items);
                        this.OnListChanged(after);
                  }
            }
            public void Clear()
            {
                  ListExChangingEventArgs<T> before = new ListExChangingEventArgs<T>(ListExChangeAction.Clear, null);
                  this.OnListChanging(before);
                  if (!before.Cancel)
                  {
                        this.lst.Clear();
                        ListExChangedEventArgs<T> after = new ListExChangedEventArgs<T>(ListExChangeAction.Clear, null);
                        this.OnListChanged(after);
                  }
            }

            public bool Contains(T item)
            {
                  return this.lst.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                  this.lst.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                  get { return this.lst.Count; }
            }

            public bool IsReadOnly
            {
                  get { return false; }
            }

            public bool Remove(T item)
            {
                  if (this.lst.Contains(item))
                  {
                        ListExChangingEventArgs<T> before = new ListExChangingEventArgs<T>(ListExChangeAction.Remove, item);
                        this.OnListChanging(before);
                        bool res = false;
                        if (!before.Cancel)
                        {
                              res = this.lst.Remove(item);
                              ListExChangedEventArgs<T> after = new ListExChangedEventArgs<T>(ListExChangeAction.Remove, item);
                              this.OnListChanged(after);
                        }
                        return res;
                  }
                  else
                  {
                        return false;
                  }
            }

            public IEnumerator<T> GetEnumerator()
            {
                  return this.lst.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                  return this.lst.GetEnumerator();
            }
            #endregion

            public bool IsFirst(T item)
            {
                  return this.IndexOf(item) == 0;
            }
            public bool IsLast(T item)
            {
                  return this.IndexOf(item) == (this.lst.Count - 1);
            }
      }
}
