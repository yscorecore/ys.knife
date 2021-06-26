using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace YS.Knife.Data.FilterExpressions.Converters
{


    internal abstract class OpExpressionConverter : ExpressionConverter
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
        protected abstract Func<Expression, Expression, BinaryExpression> ReverseCompareFunc { get; }
        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value,
            List<FilterInfo> subFilters)
        {
            if (IsNull(value)) throw new FieldInfo2ExpressionException($"Can not handle null value for {this.FilterType}.");
            var (isNullable, underlyingType) = propInfo.PropertyType.GetUnderlyingTypeTypeInfo();
            object constValue = this.ChangeType(value, underlyingType);
            if (SupportOpTypes.Contains(underlyingType))
            {// user op 
                return CompareFunc(Expression.Property(p, propInfo), Expression.Convert(Expression.Constant(constValue), propInfo.PropertyType));
            }
            else
            {// use CompareTo
                if (!typeof(IComparable<>).MakeGenericType(underlyingType).IsAssignableFrom(underlyingType))
                {
                    throw new InvalidOperationException(string.Format("{0} 只能处理实现了IComparable<{1}>接口的类型", this.FilterType, underlyingType.FullName));
                }
                var left = propInfo.VisitExpression(p);
                // reverse the expression to handle null value
                var expression = ReverseCompareFunc(Expression.Call(
                    Expression.Constant(constValue),
                    underlyingType.GetMethod("CompareTo", new Type[] { underlyingType }),
                    left), Expression.Constant(0));
                return isNullable ?
                    Expression.AndAlso(
                        Expression.Property(Expression.Property(p, propInfo), nameof(Nullable<int>.HasValue)),
                        expression)
                    : expression;
            }
        }
    }
}
