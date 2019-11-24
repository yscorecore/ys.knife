using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;

namespace System
{
    public class TNode<T>
    {
        private readonly TNodeCollection<T> _children = new TNodeCollection<T>();
        public TNode<T> Parent { get; private set; }
        public T Value { get; set; }
        public TNode(T value)
        {
            this.Value = value;
        }
        /// <summary>
        /// 获取一个值，该值反映了是否是叶子节点
        /// </summary>
        public bool IsLeaf
        {
            get { return _children == null || _children.Count == 0; }
        }
        /// <summary>
        /// 获取一个值，该值反映了是否是顶级节点
        /// </summary>
        public bool IsRoot
        {
            get { return this.Parent == null; }
        }
        public TNode<T> this[int index]
        {
            get { return _children[index]; }
        }
        public List<TNode<T>> Children
        {
            get { return _children; }
        }
        public TNode<T> AddChild(T value)
        {
            var node = new TNode<T>(value) { Parent = this };
            _children.Add(node);
            return node;
        }
        public void AddChild(TNode<T> node)
        {
            node.Parent = this;
            _children.Add(node);
        }
        public TNode<T>[] AddChildren(params T[] values)
        {
            List<TNode<T>> lst = new List<TNode<T>>();
            foreach (T value in values)
            {
                lst.Add(this.AddChild(value));
            }
            return lst.ToArray();
        }
        public bool RemoveChild(TNode<T> node)
        {
            bool res = _children.Remove(node);
            if (res)
            {
                node.Parent = null;
            }
            return res;
        }
        public void ClearChildren()
        {
            this._children.Clear();
        }
        //public void TraverseBefore(Action<T> action)
        //{
        //      if (action != null)
        //      {
        //            action(Value);
        //            foreach (var child in _children)
        //                  child.TraverseBefore(action);
        //      }
        //}
        //public void TraverseAfter(Action<T> action)
        //{
        //      if (action != null)
        //      {
        //            foreach (var child in _children)
        //                child.TraverseAfter(action);
        //            action(Value);
        //      }
        //}

        /// <summary>
        /// 获取所有的叶子节点
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TNode<T>> GetLeafNodes()
        {
            if (this._children == null || this._children.Count == 0)
            {
                yield return this;
            }
            else
            {
                foreach (TNode<T> node in this._children)
                {
                    foreach (TNode<T> value in node.GetLeafNodes())
                    {
                        yield return value;
                    }
                }
            }

        }
        /// <summary>
        /// 前序遍历树的节点
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TNode<T>> FlattenBefore()
        {
            yield return this;
            foreach (TNode<T> node in this._children)
            {
                foreach (TNode<T> value in node.FlattenBefore())
                {
                    yield return value;
                }
            }
        }
        /// <summary>
        /// 前序遍历树的值
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> FlattenBeforeValue()
        {
            yield return this.Value;
            foreach (TNode<T> node in this._children)
            {
                foreach (T value in node.FlattenBeforeValue())
                {
                    yield return value;
                }
            }
        }
        /// <summary>
        /// 后序遍历树的值的节点
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TNode<T>> FlattenAfter()
        {
            foreach (TNode<T> node in this._children)
            {
                foreach (TNode<T> value in node.FlattenAfter())
                {
                    yield return value;
                }
            }
            yield return this;
        }
        /// <summary>
        /// 后序遍历树的值
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> FlattenAfterValue()
        {
            foreach (TNode<T> node in this._children)
            {
                foreach (T value in node.FlattenAfterValue())
                {
                    yield return value;
                }
            }
            yield return this.Value;
        }


    }

    public class TNodeCollection<T> : List<TNode<T>>
    {
        /// <summary>
        /// 前序遍历树
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> FlattenBefore()
        {
            foreach (TNode<T> node in this)
            {
                foreach (T item in node.FlattenBeforeValue())
                {
                    yield return item;
                }
            }
        }
        /// <summary>
        /// 后序遍历树
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> FlattenAfter()
        {
            foreach (TNode<T> node in this)
            {
                foreach (T item in node.FlattenAfterValue())
                {
                    yield return item;
                }
            }
        }

        public void SortTree(Comparison<T> comparison)
        {
            if (comparison == null) throw new ArgumentNullException("comparison");
            var sortCompare = new SortCompare(comparison);
            this.SortInternal(this, sortCompare);
        }
        public void SortTree(IComparer<T> comparer)
        {
            this.SortTree(comparer.Compare);
        }
        public void SortTree()
        {
            this.SortInternal(this, null);
        }
        public void SortTree<TKey>(Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            if (keySelector == null)
            {
                SortTree();
            }
            else
            {
                Comparison<T> comparison = (a, b) =>
                {
                    var key1 = keySelector(a);
                    var key2 = keySelector(b);
                    return key1.CompareTo(key2);
                };
                SortTree(comparison);
            }

        }
        private void SortInternal(List<TNode<T>> lst, IComparer<TNode<T>> comparer)
        {
            if (comparer == null)
            {
                lst.Sort();
            }
            else
            {
                lst.Sort(comparer);
            }
            foreach (var childNode in lst)
            {
                SortInternal(childNode.Children, comparer);
            }
        }

        internal class SortCompare : IComparer<TNode<T>>
        {
            public SortCompare(Comparison<T> comparison)
            {
                this.comparison = comparison;
            }
            Comparison<T> comparison;
            public int Compare(TNode<T> x, TNode<T> y)
            {
                var v1 = x.Value;
                var v2 = y.Value;
                return comparison(v1, v2);
            }
        }

    }
    
}
