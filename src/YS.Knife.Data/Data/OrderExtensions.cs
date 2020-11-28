using System.Collections.Generic;
using System.Linq.Expressions;
using YS.Knife.Data;

namespace System.Linq
{
    public static class OrderExtensions
    {

        public static IQueryable<T> Order<T>(this IQueryable<T> source, OrderInfo orderInfo)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return source.Order(orderInfo?.Items ?? Enumerable.Empty<OrderItem>());
        }
        private static IQueryable<T> Order<T>(this IQueryable<T> source, IEnumerable<OrderItem> orderitems)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            HashSet<string> used = new HashSet<string>();
            IOrderedQueryable<T> result = null;
            foreach (var v in orderitems ?? Enumerable.Empty<OrderItem>())
            {
                if (!used.Contains(v.FieldName))
                {
                    used.Add(v.FieldName);
                    if (v.OrderType == OrderType.Asc)
                    { //顺序
                        if (result != null)
                        {
                            result = result.ThenAsc(v.FieldName);
                        }
                        else
                        {
                            result = source.Asc(v.FieldName);
                        }
                    }
                    else
                    {
                        if (result != null)
                        {
                            result = result.ThenDesc(v.FieldName);
                        }
                        else
                        {
                            result = source.Desc(v.FieldName);
                        }
                    }

                }
            }
            return result ?? source;
        }
        public static IQueryable<T> Order<T>(this IQueryable<T> source, params OrderItem[] orderitems)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return Order(source, orderitems as IEnumerable<OrderItem>);
        }
        private static IOrderedQueryable<T> Asc<T>(this IQueryable<T> source, string fieldName)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
            return DoOrder(source, nameof(Enumerable.OrderBy), fieldName);
        }
        private static IOrderedQueryable<T> ThenAsc<T>(this IQueryable<T> source, string fieldName)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = fieldName ?? throw new ArgumentNullException(nameof(fieldName));

            return DoOrder(source, nameof(Enumerable.ThenBy), fieldName);
        }
        private static IOrderedQueryable<T> Desc<T>(this IQueryable<T> source, string fieldName)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
            return DoOrder(source, nameof(Enumerable.OrderByDescending), fieldName);
        }
        private static IOrderedQueryable<T> ThenDesc<T>(this IQueryable<T> source, string fieldName)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
            return DoOrder(source, nameof(Enumerable.ThenByDescending), fieldName);
        }




        private static IOrderedQueryable<T> DoOrder<T>(IQueryable<T> source, string methodName, string fieldName)
        {
            ParameterExpression p = Expression.Parameter(typeof(T), "p");
            Expression exp = p;
            var type = typeof(T); ;
            foreach (var v in fieldName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries))
            {
                exp = Expression.PropertyOrField(exp, v);
                type = type.GetProperty(v).PropertyType;
            }

            var types = new Type[] { typeof(T), type };
            Expression expr = Expression.Call(typeof(Queryable),
            methodName, types, source.Expression, Expression.Lambda(exp, p));
            return source.Provider.CreateQuery<T>(expr) as IOrderedQueryable<T>;
        }


    }
}
