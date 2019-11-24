using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace System.Record
{
    /// <summary>
    /// 表示一组操作集合
    /// </summary>
    public sealed class OperationGroup:OperationBase,ICollection<OperationBase>
    {
        private List<OperationBase> m_lst = new List<OperationBase>();
        #region 构造函数
        /// <summary>
        /// 初始化<see cref="OperationGroup"/>类的新实例。
        /// </summary>
        public OperationGroup ()
        { 
        }
        /// <summary>
        /// 初始化<see cref="OperationGroup"/>类的新实例。
        /// </summary>
        public OperationGroup (IEnumerable<OperationBase> operations)
        {
            m_lst.AddRange(operations);
        }
        /// <summary>
        /// 初始化<see cref="OperationGroup"/>类的新实例。
        /// </summary>
        public OperationGroup (params OperationBase[] oprations)
        {
            m_lst.AddRange(oprations);
        }
        #endregion

        #region ICollection<IOperation> 成员

        public void Add (OperationBase item)
        {
            this.m_lst.Add(item);
        }

        public void Clear ()
        {
            this.m_lst.Clear();
        }

        public bool Contains (OperationBase item)
        {
            return this.m_lst.Contains(item);
        }

        public void CopyTo (OperationBase[] array, int arrayIndex)
        {
            this.m_lst.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.m_lst .Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove (OperationBase item)
        {
            return this.m_lst.Remove(item);
        }
        public IEnumerator<OperationBase> GetEnumerator ()
        {
            return this.m_lst.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return this.m_lst.GetEnumerator();
        }

        #endregion

        #region IOperation 成员

        public override void SetNewValue ()
        {
            foreach (OperationBase op in this.m_lst)
            {
                if (op != null)
                {
                    op.SetNewValue();
                }
            }
        }

        public override void SetOldValue ()
        {
            foreach (OperationBase op in this.m_lst)
            {
                if (op != null)
                {
                    op.SetOldValue();
                }

            }
        }

        #endregion

    }
}
