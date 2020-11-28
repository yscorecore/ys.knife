using System;
using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;
using YS.Knife.Entity.Model;

namespace YS.Knife.Entity
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1307:指定 StringComparison", Justification = "<挂起>")]

    public static class EntityStoreExtentions
    {

        private static void ForEach<T>(this IEnumerable<T> sources, Action<T> action)
        {
            foreach (var item in sources ?? Enumerable.Empty<T>())
            {
                action?.Invoke(item);
            }
        }
        public static void Add<T>(this IEntityStore<T> store, IEnumerable<T> entities)
            where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            entities.ForEach(p => store.Add(p));
        }
        public static void Delete<T>(this IEntityStore<T> store, IEnumerable<T> entities)
              where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            entities.ForEach(p => store.Delete(p));
        }
        public static void Update<T>(this IEntityStore<T> store, IEnumerable<T> entities)
              where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            entities.ForEach(p => store.Update(p));
        }
        public static void Update<T>(this IEntityStore<T> store, IEnumerable<T> entities, params string[] fields)
             where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            entities.ForEach(p => store.Update(p, fields));
        }
        public static int Count<T>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions)
             where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            return store.Query(conditions).Count();
        }
        public static long LongCount<T>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions)
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            return store.Query(conditions).LongCount();
        }
        public static TR Max<T, TR>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions, Func<T, TR> fieldSelector)
              where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            return store.Query(conditions).Max(fieldSelector);
        }
        public static TR Min<T, TR>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions, Func<T, TR> fieldSelector)
              where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            return store.Query(conditions).Max(fieldSelector);
        }

        public static decimal? Sum<T>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions, Func<T, decimal?> fieldSelector)
           where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            return store.Query(conditions).Sum(fieldSelector);
        }
        public static decimal SumOrDefault<T>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions, Func<T, decimal?> fieldSelector)
           where T : class
        {
            var res = store.Sum(conditions, fieldSelector);
            return res.HasValue ? res.Value : default;
        }
        public static int? Sum<T>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions, Func<T, int?> fieldSelector)
           where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            return store.Query(conditions).Sum(fieldSelector);
        }
        public static int SumOrDefault<T>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions, Func<T, int?> fieldSelector)
          where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            var res = store.Sum(conditions, fieldSelector);
            return res.HasValue ? res.Value : default;
        }


        //public static void BatchOperation<T>(this IEntityStore<T> store, IEnumerable<T> addDatas, IEnumerable<T> updateDatas, IEnumerable<T> deleteDatas)
        //    where T : class
        //{
        //    if (addDatas != null)
        //    {
        //        store.Add(addDatas);
        //    }
        //    if (updateDatas != null)
        //    {
        //        store.Update(updateDatas);
        //    }
        //    if (deleteDatas != null)
        //    {
        //        store.Delete(deleteDatas);
        //    }
        //}

        #region 树

        public static void AddTreeNode<T, TKey>(this IEntityStore<T> store, T treeItem, string treePathSplitChar = ".")
           where T : class, ISelfTree<TKey>
        {
            //_ = treeItem ?? throw new ArgumentNullException(nameof(treeItem));
            if (default(TKey).Equals(treeItem.ParentId))
            {
                //root node
                treeItem.Depth = 0;
                treeItem.NodePath = Hex36Convert.ConvertDecToHex36(0);
                treeItem.PathValue = 0;
                store.Add(treeItem);
            }
            else
            {
                var parent = store.FindByKey(treeItem.ParentId);
                if (parent == null)
                {
                    throw new ApplicationException("can not find parent node.");
                }
                treeItem.Depth = parent.Depth + 1;
                string prefix = parent.NodePath + treePathSplitChar;
                //fid current max value

                var currentMax = store.Max(p => p.NodePath.StartsWith(prefix), p => (int?)p.PathValue);
                var nextPathValue = (currentMax.HasValue ? currentMax.Value : 0) + 1;
                treeItem.PathValue = nextPathValue;
                treeItem.NodePath = $"{parent.NodePath}{treePathSplitChar}{Hex36Convert.ConvertDecToHex36(nextPathValue)}";
                store.Add(treeItem);
            }
        }

        #endregion

        //#region 排序
        ////改变字段的序列
        //public static void UpdateSequence<T>(this IEntityStore<T> store, params T[] entities)
        //    where T : class, ISequence
        //{
        //    store.Update(entities, Exp<ISequence>.FieldName(p => p.Sequence));
        //}
        ///// <summary>
        ///// 重新整理分配字段的序列值，按照数组的先后顺序
        ///// </summary>
        ///// <typeparam name="EFContextType"></typeparam>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="context"></param>
        ///// <param name="entities"></param>
        ///// <param name="start"></param>
        ///// <param name="Step"></param>
        //public static void TrimSequence<T>(this IEntityStore<T> store, T[] entities, int start = 100, int Step = 100)
        //   where T : class, ISequence
        //{
        //    if (entities == null || entities.Length == 0) return;
        //    for (int i = 0; i < entities.Length; i++)
        //    {
        //        entities[i].Sequence = start + i * Step;
        //    }
        //    store.UpdateSequence(entities);
        //}
        //#endregion

        #region Single

        public static T FindSingle<T>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions)
              where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            var query = store.Query(conditions);
            return query.Single();
        }
        public static TR FindSingle<T, TR>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions, Func<IQueryable<T>, IQueryable<TR>> selector)
             where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            var query = store.Query(conditions);
            return selector(query).Single();
        }
        public static T FindSingleOrDefault<T>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions)
                 where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            var query = store.Query(conditions);
            return query.SingleOrDefault();
        }
        public static TR FindSingleOrDefault<T, TR>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions, Func<IQueryable<T>, IQueryable<TR>> selector)
             where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            var query = store.Query(conditions);
            return selector(query).SingleOrDefault();
        }
        #endregion

        #region First
        public static T FindFirst<T>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions)
              where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            var query = store.Query(conditions);
            return query.First();
        }
        public static TR FindFirst<T, TR>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions, Func<IQueryable<T>, IQueryable<TR>> selector)
             where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            var query = store.Query(conditions);
            return selector(query).First();
        }
        public static T FindFirstOrDefault<T>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions)
                 where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            var query = store.Query(conditions);
            return query.FirstOrDefault();
        }
        public static TR FindFirstOrDefault<T, TR>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions, Func<IQueryable<T>, IQueryable<TR>> selector)
             where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            var query = store.Query(conditions);
            return selector(query).FirstOrDefault();
        }
        #endregion

        #region Last
        public static T FindLast<T>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions)
              where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            var query = store.Query(conditions);
            return query.Last();
        }
        public static TR FindLast<T, TR>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions, Func<IQueryable<T>, IQueryable<TR>> selector)
             where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            var query = store.Query(conditions);
            return selector(query).Last();
        }
        public static T FindLastOrDefault<T>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions)
                 where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            var query = store.Query(conditions);
            return query.LastOrDefault();
        }
        public static TR FindLastOrDefault<T, TR>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions, Func<IQueryable<T>, IQueryable<TR>> selector)
             where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            var query = store.Query(conditions);
            return selector(query).LastOrDefault();
        }

        #endregion

        //#region ListAll
        //public static List<T> ListAll<T>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions, IEnumerable<OrderItem> orderItems)
        //     where T : class
        //{
        //    var query = store.Query(conditions);
        //    return query.Order(orderItems).ToList();
        //}
        //public static List<TR> ListAll<T, TR>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions, IEnumerable<OrderItem> orderItems, Func<IQueryable<T>, IQueryable<TR>> selector)
        //    where T : class
        //{
        //    if (selector == null) throw new ArgumentNullException("selector");
        //    var query = store.Query(conditions);

        //    return selector(query.Order(orderItems)).ToList();
        //}
        //#endregion

        //#region ListPage
        //public static PageData<T> ListPage<T>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions, IEnumerable<OrderItem> orderItems, int pageIndex, int pageSize)
        //         where T : class
        //{
        //    if (orderItems == null) throw new ArgumentNullException(nameof(orderItems));
        //    if (orderItems.Count() <= 0) throw new ArgumentException(nameof(orderItems), "至少包含一个排序字段");
        //    if (pageSize <= 0) throw new ArgumentOutOfRangeException("pageSize", "pageSize必须大于0");
        //    if (pageIndex < 0) throw new ArgumentOutOfRangeException("pageIndex", "pageIndex必须为正数");
        //    var query = store.Query(conditions);
        //    var sortedQuery = query.Order(orderItems);
        //    return new PageData<T>(sortedQuery, pageIndex, pageSize);
        //}

        //public static PageData<TR> ListPage<T, TR>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions, IEnumerable<OrderItem> orderItems, Func<IQueryable<T>, IQueryable<TR>> selector, int pageIndex, int pageSize)
        //      where T : class
        //{
        //    if (selector == null) throw new ArgumentNullException("selector");
        //    if (orderItems == null) throw new ArgumentNullException(nameof(orderItems));
        //    if (orderItems.Count() <= 0) throw new ArgumentException(nameof(orderItems), "至少包含一个排序字段");
        //    if (pageSize <= 0) throw new ArgumentOutOfRangeException("pageSize", "pageSize必须大于0");
        //    if (pageIndex < 0) throw new ArgumentOutOfRangeException("pageIndex", "pageIndex必须为正数");
        //    var query = store.Query(conditions);
        //    var sortedQuery = selector(query).Order(orderItems);
        //    return new PageData<TR>(sortedQuery, pageIndex, pageSize);
        //}
        //#endregion

        //#region ListLimit
        //public static LimitData<T> ListLimit<T>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions, IEnumerable<OrderItem> orderItems, int offset, int limit)
        // where T : class
        //{
        //    if (orderItems == null) throw new ArgumentNullException(nameof(orderItems));
        //    if (orderItems.Count() <= 0) throw new ArgumentException(nameof(orderItems), "至少包含一个排序字段");
        //    if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), $"{nameof(limit)}必须大于0");
        //    if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), $"{nameof(offset)}必须为正数或零");
        //    var query = store.Query(conditions);
        //    var sortedQuery = query.Order(orderItems);
        //    return new LimitData<T>(sortedQuery, offset, limit);
        //}

        //public static LimitData<TR> ListLimit<T, TR>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions, IEnumerable<OrderItem> orderItems, Func<IQueryable<T>, IQueryable<TR>> selector, int offset, int limit)
        //      where T : class
        //{
        //    if (selector == null) throw new ArgumentNullException(nameof(selector));
        //    if (orderItems == null) throw new ArgumentNullException(nameof(orderItems));
        //    if (orderItems.Count() <= 0) throw new ArgumentException(nameof(orderItems), "必须有排序字段");
        //    if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), $"{nameof(limit)}必须大于0");
        //    if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), $"{nameof(offset)}必须为正数或零");
        //    var query = store.Query(conditions);
        //    var sortedQuery = selector(query).Order(orderItems);
        //    return new LimitData<TR>(sortedQuery, offset, limit);
        //}
        //#endregion
        #region DeleteByCondition
        public static void Delete<T>(this IEntityStore<T> store, Expression<Func<T, bool>> conditions)
            where T : class
        {
            _ = store ?? throw new ArgumentNullException(nameof(store));
            var list = store.Query(conditions).ToList();
            store.Delete(list);
        }
        #endregion
    }
}
