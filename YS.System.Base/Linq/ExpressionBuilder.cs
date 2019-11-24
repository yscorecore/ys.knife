using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace System.Linq
{
    /// <summary>
    /// 表示表达式的创建器
    /// </summary>
    public static class PredicateExtensions
    {
        /// <summary>
        /// 表示恒真表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> True<T>() { return f => true; }
        /// <summary>
        /// 表示恒假表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> False<T>() { return f => false; }
        /// <summary>
        /// 将两个表达式做Or运算
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> expression1,
           Expression<Func<T, bool>> expression2)
        {
            var parameter = Expression.Parameter(typeof(T),"p");

            //var leftVisitor = new ReplaceExpressionVisitor(expression1.Parameters[0], parameter);
            //var left = leftVisitor.Visit(expression1.Body);

            //var rightVisitor = new ReplaceExpressionVisitor(expression2.Parameters[0], parameter);
            //var right = rightVisitor.Visit(expression2.Body);

            //return Expression.Lambda<Func<T, bool>>(
            //    Expression.OrElse(left, right), parameter);
            return null;
        }
        /// <summary>
        /// 将两个表达式做And运算
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expression1,
              Expression<Func<T, bool>> expression2)
        {
            var parameter = Expression.Parameter(typeof(T),"p");

            //var leftVisitor = new ReplaceExpressionVisitor(expression1.Parameters[0], parameter);
            //var left = leftVisitor.Visit(expression1.Body);

            //var rightVisitor = new ReplaceExpressionVisitor(expression2.Parameters[0], parameter);
            //var right = rightVisitor.Visit(expression2.Body);

            //return Expression.Lambda<Func<T, bool>>(
            //    Expression.AndAlso(left, right), parameter);
            return null;
           
        }

        //private class ReplaceExpressionVisitor
        //: ExpressionVisitor
        //{
        //    private readonly Expression _oldValue;
        //    private readonly Expression _newValue;

        //    public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        //    {
        //        _newValue = newValue;
        //        _oldValue = oldValue;
        //    }

        //    public override Expression Visit(Expression node)
        //    {
        //        if (object.ReferenceEquals( node , _oldValue))
        //            return _newValue;
        //        return base.Visit(node);
        //    }
        //}
    }


}
