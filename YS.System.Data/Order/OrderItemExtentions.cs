using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace System.Data
{
    public static class OrderItemExtentions
    {
        public static IQueryable<T> Order<T>(this IQueryable<T> source, IEnumerable<OrderItem> orderitems)
        {
            HashSet<string> used = new HashSet<string>();
            IOrderedQueryable<T> result = null;
            foreach (var v in orderitems ?? new OrderItem[0])
            {
                if (!used.Contains(v.FieldName))
                {
                    used.Add(v.FieldName);
                    if (v.OrderType == OrderType.ASC)
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
            return Order<T>(source, orderitems as IEnumerable<OrderItem>);
        }
        public static IOrderedQueryable<T> Asc<T>(this IQueryable<T> source, string fieldName)
        {
            return DoOrder(source, "OrderBy", fieldName);
        }
        public static IOrderedQueryable<T> ThenAsc<T>(this IQueryable<T> source, string fieldName)
        {
            return DoOrder(source, "ThenBy", fieldName);
        }
        public static IOrderedQueryable<T> Desc<T>(this IQueryable<T> source, string fieldName)
        {
            return DoOrder(source, "OrderByDescending", fieldName);
        }
        public static IOrderedQueryable<T> ThenDesc<T>(this IQueryable<T> source, string fieldName)
        {
            return DoOrder(source, "ThenByDescending", fieldName);
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
