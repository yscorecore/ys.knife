using System;
using System.Collections.Generic;
using System.Text;

namespace System.Record
{
    /// <summary>
    /// 对顺序表的操作
    /// </summary>
    public class ListOperation<T>:OperationBase
    {
        private readonly IList<T> m_listObject;
        private readonly T m_itemOjbect;
        private readonly ListOperationType m_operationType;
        private readonly int m_oldIndex;
        private readonly int m_newIndex;
        public ListOperation (IList<T> listObject,T itemObject, ListOperationType operationType,int oldIndex,int newIndex)
        {
            if (listObject == null) throw new ArgumentNullException("listObject");
            this.m_listObject = listObject;
            this.m_itemOjbect = itemObject;
            this.m_operationType = operationType;
            this.m_oldIndex = oldIndex;
            this.m_newIndex = newIndex;
            
        }
        public override void SetNewValue ()
        {
            switch (this.m_operationType)
            { 
                case ListOperationType .Add :
                    this.m_listObject.Add(m_itemOjbect);
                    break;
                case ListOperationType .Remove :
                    this.m_listObject.Remove(m_itemOjbect);
                    break;
                case ListOperationType .Insert :
                    this.m_listObject.Insert(m_newIndex,m_itemOjbect);
                    break;
                case ListOperationType .BringForward :
                    if (this.m_listObject.Remove(m_itemOjbect))
                    {
                        this.m_listObject.Insert(m_newIndex, m_itemOjbect);
                    };
                    break;
                case ListOperationType .BringToFront :
                    if (this.m_listObject.Remove(m_itemOjbect))
                    {
                        this.m_listObject.Insert(m_newIndex, m_itemOjbect);
                    };
                    break;
                case ListOperationType .SendBackward :
                    if (this.m_listObject.Remove(m_itemOjbect))
                    {
                        this.m_listObject.Insert(m_newIndex, m_itemOjbect);
                    };
                    break;
                case ListOperationType .SendToBack :
                    if (this.m_listObject.Remove(m_itemOjbect))
                    {
                        this.m_listObject.Insert(m_newIndex, m_itemOjbect);
                    };
                    break;
            }
        }
        public override  void SetOldValue ()
        {
            switch (this.m_operationType)
            {
                case ListOperationType.Add:
                    this.m_listObject.Remove(m_itemOjbect);
                    break;
                case ListOperationType.Remove:
                    this.m_listObject.Insert(m_oldIndex, m_itemOjbect);
                    break;
                case ListOperationType.Insert:
                    this.m_listObject.Remove(m_itemOjbect);
                    break;
                case ListOperationType.BringForward:
                    if (this.m_listObject.Remove(m_itemOjbect))
                    {
                        this.m_listObject.Insert(m_oldIndex, m_itemOjbect);
                    };
                    break;
                case ListOperationType.BringToFront:
                    if (this.m_listObject.Remove(m_itemOjbect))
                    {
                        this.m_listObject.Insert(m_oldIndex,m_itemOjbect);
                    };
                    break;
                case ListOperationType.SendBackward:
                    if (this.m_listObject.Remove(m_itemOjbect))
                    {
                        this.m_listObject.Insert(m_oldIndex, m_itemOjbect);
                    };
                    break;
                case ListOperationType.SendToBack:
                    if (this.m_listObject.Remove(m_itemOjbect))
                    {
                        this.m_listObject.Insert(m_oldIndex, m_itemOjbect);
                    };
                    break;
            }
          
        }
    }
    public enum ListOperationType
    {
        /// <summary>
        /// 增加一个元素
        /// </summary>
        Add,
        /// <summary>
        /// 移除一个元素
        /// </summary>
        Remove,
        /// <summary>
        /// 插入一个元素
        /// </summary>
        Insert,
        /// <summary>
        /// 将指定的元素置于顶层。
        /// </summary>
        BringToFront,
        /// <summary>
        /// 将指定的元素上移一层。
        /// </summary>
        BringForward,
        /// <summary>
        /// 将指定的元素下移一层。
        /// </summary>
        SendBackward,
        /// <summary>
        /// 将指定的元素置于底层。
        /// </summary>
        SendToBack
    }
}
