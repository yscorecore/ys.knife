using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;

namespace System
{
    public static class TreeEx
    {
        public static TNodeCollection<T> BuildTree<T>(this IEnumerable<T> items, Func<T, object> idFinder, Func<T, object> parentIdFinder)
        {
            if (idFinder == null) throw new ArgumentNullException("idFinder");
            if (parentIdFinder == null) throw new ArgumentNullException("parentIdFinder");
            if (items == null) return new TNodeCollection<T>();
            Dictionary<object, NodeEntry<TNode<T>>> dic = new Dictionary<object, NodeEntry<TNode<T>>>();
            foreach (T item in items)
            {
                NodeEntry<TNode<T>> key = new NodeEntry<TNode<T>>();
                key.CurrentID = idFinder(item);
                key.ParentID = parentIdFinder(item);
                key.Item = new TNode<T>(item);
                dic.Add(key.CurrentID, key);
            }
            foreach (KeyValuePair<object, NodeEntry<TNode<T>>> item in dic)
            {
                if (!Convert.IsDBNull(item.Value.ParentID) && item.Value.ParentID != null && dic.ContainsKey(item.Value.ParentID))
                {
                    dic[item.Value.ParentID].Item.AddChild(item.Value.Item);
                    item.Value.Flag = true;//已经处理过了
                }
            }
            TNodeCollection<T> forest = new TNodeCollection<T>();
            foreach (NodeEntry<TNode<T>> value in dic.Values)
            {
                if (value.Flag == false)    //没有处理过，说明是顶级节点
                {
                    forest.Add(value.Item);
                }
            }
            return forest;
        }
        public static TNodeCollection<T> BuildTree<T,TOrderKey>(this IEnumerable<T> items, Func<T, object> idFinder, Func<T, object> parentIdFinder,Func<T,TOrderKey> orderField)
            where TOrderKey:IComparable<TOrderKey>
        {
            var nodes = items.BuildTree(idFinder, parentIdFinder);
            if(orderField!=null)
            {
                nodes.SortTree(orderField);
            }
            return nodes;
        }
        public static TNodeCollection<T> BuildTree<T, TOrderKey>(this IEnumerable<T> items, Func<T, object> idFinder, Func<T, object> parentIdFinder, Comparison<T> comparison)
        {
            var nodes = items.BuildTree(idFinder, parentIdFinder);
            if (comparison != null)
            {
                nodes.SortTree(comparison);
            }
            return nodes;
        }
        [Obsolete()]
        public static TNodeCollection<T> BuildTree<T>(this IEnumerable<T> items, string selfFlag, string parentFlag)
        {
            if (items == null) return new TNodeCollection<T>();

            Type itemType = typeof(T);
            PropertyInfo selfInfo = itemType.GetProperty(selfFlag);
            PropertyInfo parentInfo = itemType.GetProperty(parentFlag);
            Dictionary<object, NodeEntry<TNode<T>>> dic = new Dictionary<object, NodeEntry<TNode<T>>>();
            foreach (T item in items)
            {
                NodeEntry<TNode<T>> key = new NodeEntry<TNode<T>>();
                key.CurrentID = selfInfo.GetValue(item, null);
                key.ParentID = parentInfo.GetValue(item, null);
                key.Item = new TNode<T>(item);
                dic.Add(key.CurrentID, key);
            }
            foreach (KeyValuePair<object, NodeEntry<TNode<T>>> item in dic)
            {
                if (!Convert.IsDBNull(item.Value.ParentID) && item.Value.ParentID != null && dic.ContainsKey(item.Value.ParentID))
                {
                    dic[item.Value.ParentID].Item.AddChild(item.Value.Item);
                    item.Value.Flag = true;//已经处理过了
                }
            }
            TNodeCollection<T> forest = new TNodeCollection<T>();
            foreach (NodeEntry<TNode<T>> value in dic.Values)
            {
                if (value.Flag == false)    //没有处理过，说明是顶级节点
                {
                    forest.Add(value.Item);
                }
            }
            return forest;
        }
        public static R ConvertTree<T, R>(this TNode<T> node, Func<T, R> selecter, Func<R, IList> childrenFinder)
        {
            if (childrenFinder == null) throw new ArgumentNullException("childrenFinder");
            var r = selecter(node.Value);
            IList lst = childrenFinder(r);
            if (lst != null)
            {
                foreach (var c in ConvertTree(node.Children, selecter, childrenFinder))
                {
                    lst.Add(c);
                }
            }
            return r;
        }
        public static IEnumerable<R> ConvertTree<T, R>(this IEnumerable<TNode<T>> nodes, Func<T, R> selecter, Func<R, IList> childrenFinder)
        {
            if (childrenFinder == null) throw new ArgumentNullException("childrenFinder");
            foreach (var v in nodes)
            {
                yield return ConvertTree(v, selecter, childrenFinder);
            }
        }
        public static IEnumerable<T> ExpandTree<T>(this IEnumerable<T> nodes, Func<T, IEnumerable<T>> childrenFinder, Func<T, bool> filter)
        {
            if (childrenFinder == null) throw new ArgumentNullException("childrenFinder");

            foreach (var n in nodes ?? (IEnumerable<T>)(new T[0]))
            {
                if (filter != null)
                {
                    if (filter(n))
                    {
                        yield return n;
                    }
                }
                else
                {
                    yield return n;
                }

                foreach (var c in ExpandTree(childrenFinder(n), childrenFinder, filter))
                {
                    yield return c;
                }
            }
        }
        public static IEnumerable<T> ExpandTree<T>(this T node, Func<T, IEnumerable<T>> childrenFinder, Func<T, bool> filter)
        {
            if (childrenFinder == null) throw new ArgumentNullException("childrenFinder");
            if (filter != null)
            {
                if (filter(node))
                {
                    yield return node;
                }
            }
            else
            {
                yield return node;
            }
            foreach (var c in ExpandTree(childrenFinder(node), childrenFinder, filter))
            {
                yield return c;
            }
        }

        
        public class NodeEntry<T>
        {
            public object CurrentID;
            public object ParentID;
            public T Item;
            public bool Flag;
        }
    }
}
