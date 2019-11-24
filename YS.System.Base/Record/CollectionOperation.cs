using System;
using System.Collections.Generic;
using System.Text;

namespace System.Record
{
    /// <summary>
    /// 表示对集合的操作
    /// </summary>
    public class CollectionOperation<T> : OperationBase
    {
        #region IOperation 成员

        private readonly ICollection<T> m_collectionObject;

        public ICollection<T> CollectionObject {
            get { return m_collectionObject; }
        }

        private readonly T m_itemObject;

        public T ItemObject {
            get { return m_itemObject; }
        }

        private readonly CollectionOperationType m_operationType;

        public CollectionOperationType OperationType {
            get { return m_operationType; }
        }

        public CollectionOperation (ICollection<T> collectionObject, T itemObject, CollectionOperationType operationType) {
            if (collectionObject == null) throw new ArgumentNullException("collectionObject");
            this.m_collectionObject = collectionObject;
            this.m_itemObject = itemObject;
            this.m_operationType = operationType;
            if (operationType == CollectionOperationType.Add) this.Description = "给集合中添加元素";
            else this.Description = "移除集合中的元素";
        }
        /// <summary>
        /// 设置新值。
        /// </summary>
        public override void SetNewValue () {
            if (this.m_operationType == CollectionOperationType.Add) {
                this.m_collectionObject.Remove(m_itemObject);
            }
            else {
                this.m_collectionObject.Add(m_itemObject);
            }
        }

        /// <summary>
        /// 设置旧值。
        /// </summary>
        public override void SetOldValue () {
            if (this.m_operationType == CollectionOperationType.Add) {
                this.m_collectionObject.Add(m_itemObject);
            }
            else {
                this.m_collectionObject.Remove(m_itemObject);
            }
        }

        #endregion
    }

    /// <summary>
    /// 表示对集合操作的类型枚举。
    /// </summary>
    public enum CollectionOperationType
    {
        /// <summary>
        /// 增加一个元素
        /// </summary>
        Add,
        /// <summary>
        /// 移除一个元素
        /// </summary>
        Remove
    }
}
