using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using YS.Knife.Data.Query.Operators;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.Query.Operators
{
    abstract class ComparableOperator : SampleTypeOperator
    {
        private static readonly Type[] SupportOpTypes = new Type[]
        {
            typeof(byte),
            typeof(short),
            typeof(int),
            typeof(double),
            typeof(float),
            typeof(decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(sbyte),
            typeof(uint),
            typeof(ulong),
            typeof(ushort)
        };
        protected abstract Func<Expression, Expression, BinaryExpression> CompareFunc { get; }
        protected override Expression CompareValue(Expression left, Expression right, Type type)
        {
            var dataType = Nullable.GetUnderlyingType(type) ?? type;
            var comparableType = typeof(IComparable<>).MakeGenericType(dataType);
            if (!comparableType.IsAssignableFrom(dataType))
            {
                throw FilterErrors.TheOperatorCanOnlyUserForComparableType(this.Operator);
            }
            if (SupportOpTypes.Contains(dataType))
            {
                return CompareFunc(left, right);
            }
            else
            {
                var method = comparableType.GetMethod(nameof(IComparable.CompareTo), new Type[] { dataType });
                return CompareFunc(Expression.Call(left, method, right), Expression.Constant(0));
            }

        }
    }
}
