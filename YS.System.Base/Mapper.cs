using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Data;

namespace System
{
    public class Mapper
    {
        public static Expression<Func<TIn,TOut>> MapExpression<TIn,TOut>()
        {
            var paramExpression=Expression.Parameter(typeof(TIn),"p");
            List<MemberBinding> bindExpressions = new List<MemberBinding>();
            foreach (var p in typeof(TOut).GetProperties())
            {
                if (p.CanWrite) continue;

                bindExpressions.Add(Expression.Bind(p, Expression.Property(paramExpression, typeof(TIn).GetProperty(p.Name))));



            }

            var initExpression = Expression.MemberInit(Expression.New(typeof(TOut)), bindExpressions.ToArray());

            return Expression.Lambda<Func<TIn, TOut>>(initExpression, paramExpression);
        }

    }
}
