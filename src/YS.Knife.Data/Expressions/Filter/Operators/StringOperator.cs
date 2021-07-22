using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace YS.Knife.Data.Filter.Operators
{
    abstract class StringOperator : SampleTypeOperator
    {

        protected abstract string MethodName { get; }


        private Lazy<MethodInfo> Method => new Lazy<MethodInfo>(() =>
           typeof(string).GetMethod(MethodName, new Type[] { typeof(string) }), true
        );

        protected override Expression CompareValue(Expression left, Expression right, Type type)
        {
            if (type != typeof(string))
            {
                throw ExpressionErrors.TheOperatorCanOnlyUserForStringType(this.Operator);
            }
            return Expression.Call(left, Method.Value, right);
        }
    }
}
