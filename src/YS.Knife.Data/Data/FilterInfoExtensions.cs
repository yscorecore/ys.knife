using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CompareFunc = System.Func<System.Linq.Expressions.Expression, System.Linq.Expressions.Expression, System.Linq.Expressions.BinaryExpression>;
namespace YS.Knife.Data
{
    public static class FilterInfoExtensions
    {
        private static bool IsNullableType(this Type type)
        {
            if (type == null) return false;
            return Nullable.GetUnderlyingType(type) != null;
        }
        private static (bool IsNullbale, Type UnderlyingType) GetUnderlyingTypeTypeInfo(this Type type)
        {
            if (type.IsNullableType())
            {
                return (true, Nullable.GetUnderlyingType(type));
            }
            else
            {
                return (false, type);
            }
        }
        private static (bool IsNullbale, Type UnderlyingType) GetUnderlyingTypeTypeInfo(this PropertyInfo propertyInfo)
        {
            return GetUnderlyingTypeTypeInfo(propertyInfo.PropertyType);
        }
        private static Expression VisitExpression(this PropertyInfo propertyInfo, Expression p)
        {
            var left = Expression.Property(p, propertyInfo);
            if (propertyInfo.PropertyType.IsNullableType())
                left = Expression.Property(left, nameof(Nullable<int>.Value));
            return left;
        }
        public static IQueryable<T> WhereCondition<T>(this IQueryable<T> source, FilterInfo filterInfo)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            if (filterInfo == null) return source;
            return source.Where(filterInfo.CreatePredicate<T>());
        }
        public static IQueryable<T> WhereCondition<T>(this IQueryable<T> source, params FilterInfo[] filterInfos)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            foreach (var item in filterInfos ?? Enumerable.Empty<FilterInfo>())
            {
                source = WhereCondition(source, item);
            }
            return source;
        }
        public static Expression<Func<T, bool>> CreatePredicate<T>(this FilterInfo filterInfo)
        {
            _ = filterInfo ?? throw new ArgumentNullException(nameof(filterInfo));
            var p = Expression.Parameter(typeof(T), "p");
            var res = FromContidtionInternal(typeof(T), filterInfo, p);
            return Expression.Lambda<Func<T, bool>>(res, p);
        }
        private static Expression FromOrConditionInternal(Type entityType, FilterInfo orCondition, ParameterExpression p)
        {
            if (orCondition == null) throw new ArgumentNullException(nameof(orCondition));
            Expression current = Expression.Constant(false);
            foreach (FilterInfo item in orCondition.Items)
            {
                var next = FromContidtionInternal(entityType, item, p);
                current = Expression.OrElse(current, next);
            }
            return current;
        }
        private static Expression FromAndConditionInternal(Type entityType, FilterInfo andCondition, ParameterExpression p)
        {
            if (andCondition == null) throw new ArgumentNullException(nameof(andCondition));
            Expression current = Expression.Constant(true);
            foreach (FilterInfo item in andCondition.Items)
            {
                var next = FromContidtionInternal(entityType, item, p);
                current = Expression.AndAlso(current, next);
            }
            return current;
        }
        private static Expression FromItemConditionItemInternal(Type entityType, FilterInfo singleItem, ParameterExpression p)
        {
            if (singleItem == null) throw new ArgumentNullException(nameof(singleItem));
            if (string.IsNullOrEmpty(singleItem.FieldName)) throw new ArgumentException("FieldName必须填充");
            var paths = singleItem.FieldName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            PropertyInfo pinfo = null;
            Type ty = entityType;
            Expression exp = p;
            for (int i = 0; i < paths.Length - 1; i++)
            {
                pinfo = ty.GetProperty(paths[i]);
                if (pinfo == null) throw new ArgumentException(string.Format("在类型{0}中无法找到指定的属性{1}", ty.FullName, paths[i]));
                ty = pinfo.PropertyType;
                exp = Expression.Property(exp, pinfo);
            }
            pinfo = ty.GetProperty(paths.Last());
            if (pinfo == null) throw new ArgumentException(string.Format("在类型{0}中无法找到指定的属性{1}", ty.FullName, paths.Last()));
            var val = singleItem.Value;
            if (val == DBNull.Value) val = null;//忽略dbnull.value
            var converter = GetConvertByFilterType(singleItem.FilterType);
            return converter.ConvertValue(exp, pinfo, val);
        }
        private static Expression FromContidtionInternal(Type entityType, FilterInfo filterInfo, ParameterExpression p)
        {
            if (filterInfo.OpType == OpType.SingleItem)
            {
                return FromItemConditionItemInternal(entityType, filterInfo, p);
            }
            else if (filterInfo.OpType == OpType.AndItems)
            {
                return FromAndConditionInternal(entityType, filterInfo, p);
            }
            else if (filterInfo.OpType == OpType.OrItems)
            {
                return FromOrConditionInternal(entityType, filterInfo, p);
            }
            else
            {
                throw new ArgumentException("unknow filterInfo");
            }
        }
        private static ExpressionConverter GetConvertByFilterType(FilterType searchType)
        {
            var instance = CreateConverterInstance(searchType);
            instance.FilterType = searchType;
            return instance;
        }
        private static ExpressionConverter CreateConverterInstance(FilterType searchType)
        {
            switch (searchType)
            {
                case FilterType.Equals:
                    return new EqualExpressionConverter();
                case FilterType.NotEquals:
                    return new NotEqualExpressionConverter();
                case FilterType.GreaterThan:
                    return new GreaterThanExpressionConverter();
                case FilterType.LessThanOrEqual:
                    return new LessThanOrEqualExpressionConverter();
                case FilterType.LessThan:
                    return new LessThanExpressionConverter();
                case FilterType.GreaterThanOrEqual:
                    return new GreaterThanOrEqualExpressionConverter();
                case FilterType.Between:
                    return new BetweenExpressionConverter();
                case FilterType.NotBetween:
                    return new NotBetweenExpressionConverter();
                case FilterType.In:
                    return new InExpressionConverter();
                case FilterType.NotIn:
                    return new NotInExpressionConverter();
                case FilterType.StartsWith:
                    return new StartWithExpressionConverter();
                case FilterType.NotStartsWith:
                    return new NotStartWithExpressionConverter();
                case FilterType.Contains:
                    return new ContainsExpressionConverter();
                case FilterType.NotContains:
                    return new NotContainsExpressionConverter();
                case FilterType.EndsWith:
                    return new EndWidhExpressionConverter();
                case FilterType.NotEndsWith:
                    return new NotEndWithExpressionConverter();
                default:
                    throw new ArgumentException(string.Format("无效的类型{0}", searchType));
            }
        }
        abstract class ExpressionConverter
        {
            public FilterType FilterType { get; set; }
            public abstract Expression ConvertValue(Expression p, PropertyInfo propInfo, object value);
            protected object ChangeType(object value, Type changeType)
            {
                //  if (value != null && value.GetType() == changeType) return value;
                //var convert = System.ComponentModel.TypeDescriptor.GetConverter(changeType);
                //return convert.ConvertFrom(value);
                if (changeType.IsEnum)
                {
                    return Enum.ToObject(changeType, value);
                }
                else
                {
                    return Convert.ChangeType(value, changeType, CultureInfo.InvariantCulture);
                }
            }
            protected string ChangeToString(object value)
            {
                return ChangeType(value, typeof(string)) as string;
            }
            protected bool IsNull(object obj)
            {
                return obj == null || obj == DBNull.Value;
            }
        }
        class EqualExpressionConverter : ExpressionConverter
        {
            private Expression ConvertNullValue(Expression p, PropertyInfo propInfo)
            {
                if (propInfo.PropertyType.IsNullableType())
                {
                    var left = Expression.Property(p, propInfo);
                    var right = Expression.Convert(Expression.Constant(null), propInfo.PropertyType);
                    return Expression.Equal(left, right);
                }
                else
                {
                    if (propInfo.PropertyType.IsValueType)
                    {
                        throw new FilterInfoExpressionException($"Can not convert null value to '{propInfo.PropertyType.FullName}' type");
                    }
                    else
                    {
                        var left = Expression.Property(p, propInfo);
                        var right = Expression.Constant(null);
                        return Expression.Equal(left, right);
                    }
                }
            }
            private Expression ConvertNotNullValue(Expression p, PropertyInfo propInfo, object value)
            {
                if (propInfo.PropertyType.IsNullableType())
                {
                    var type = Nullable.GetUnderlyingType(propInfo.PropertyType);
                    var left = Expression.Property(p, propInfo);
                    var right = Expression.Convert(Expression.Constant(this.ChangeType(value, type)), propInfo.PropertyType);
                    return Expression.Equal(left, right);
                }
                else
                {
                    var left = Expression.Property(p, propInfo);
                    var right = Expression.Constant(this.ChangeType(value, propInfo.PropertyType));
                    return Expression.Equal(left, right);
                }
            }
            public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
            {
                if (value == null)
                {
                    return ConvertNullValue(p, propInfo);
                }
                else
                {
                    return ConvertNotNullValue(p, propInfo, value);
                }
            }
        }
        class NotEqualExpressionConverter : ExpressionConverter
        {
            private Expression ConvertNullValue(Expression p, PropertyInfo propInfo)
            {
                if (propInfo.PropertyType.IsNullableType())
                {
                    var left = Expression.Property(p, propInfo);
                    var right = Expression.Convert(Expression.Constant(null), propInfo.PropertyType);
                    return Expression.NotEqual(left, right);
                }
                else
                {
                    if (propInfo.PropertyType.IsValueType)
                    {
                        throw new InvalidCastException(string.Format("无法将null对象转换为{0}类型", propInfo.PropertyType.FullName));
                    }
                    else
                    {
                        var left = Expression.Property(p, propInfo);
                        var right = Expression.Constant(null);
                        return Expression.NotEqual(left, right);
                    }
                }
            }
            private Expression ConvertNotNullValue(Expression p, PropertyInfo propInfo, object value)
            {
                if (propInfo.PropertyType.IsNullableType())
                {
                    var type = Nullable.GetUnderlyingType(propInfo.PropertyType);
                    var left = Expression.Property(p, propInfo);
                    var right = Expression.Convert(Expression.Constant(this.ChangeType(value, type)), propInfo.PropertyType);
                    return Expression.NotEqual(left, right);
                }
                else
                {
                    var left = Expression.Property(p, propInfo);
                    var right = Expression.Constant(this.ChangeType(value, propInfo.PropertyType));
                    return Expression.NotEqual(left, right);
                }
            }
            public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
            {
                if (value == null)
                {
                    return ConvertNullValue(p, propInfo);
                }
                else
                {
                    return ConvertNotNullValue(p, propInfo, value);
                }
            }
        }
        abstract class StringExpressionConverter : ExpressionConverter
        {
            protected abstract string MethodName { get; }
            public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
            {
                if (IsNull(value)) throw new InvalidOperationException(string.Format("{0} 无法处理null值", this.FilterType));
                if (propInfo.PropertyType != typeof(string)) throw new InvalidOperationException(string.Format("{0} 只适用于string类型", this.FilterType));
                string val = ChangeToString(value);
                return Expression.AndAlso(
                                Expression.NotEqual(Expression.Property(p, propInfo), Expression.Constant(null)),
                                 Expression.Call(
                                         Expression.Property(p, propInfo),
                                         typeof(string).GetMethod(MethodName, new Type[] { typeof(string) }),
                                         Expression.Constant(val)
                                         ));
            }
        }
        class ContainsExpressionConverter : StringExpressionConverter
        {
            protected override string MethodName => nameof(string.Contains);
        }
        class NotContainsExpressionConverter : ContainsExpressionConverter
        {
            public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
            {
                return Expression.Not(base.ConvertValue(p, propInfo, value));
            }
        }
        class StartWithExpressionConverter : StringExpressionConverter
        {
            protected override string MethodName => nameof(string.StartsWith);
        }
        class NotStartWithExpressionConverter : StartWithExpressionConverter
        {
            public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
            {
                return Expression.Not(base.ConvertValue(p, propInfo, value));
            }
        }
        class EndWidhExpressionConverter : StringExpressionConverter
        {
            protected override string MethodName => nameof(string.EndsWith);
        }
        class NotEndWithExpressionConverter : EndWidhExpressionConverter
        {
            public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
            {
                return Expression.Not(base.ConvertValue(p, propInfo, value));
            }
        }
        abstract class OpExpressionConverter : ExpressionConverter
        {
            public static Type[] SupportOpTypes = new Type[]
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
            protected abstract CompareFunc CompareFunc { get; }
            protected abstract CompareFunc ReverseCompareFunc { get; }
            public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
            {
                if (IsNull(value)) throw new FilterInfoExpressionException($"Can not handle null value for {this.FilterType}.");
                var nullableTypeInfo = propInfo.GetUnderlyingTypeTypeInfo();
                object constValue = this.ChangeType(value, nullableTypeInfo.UnderlyingType);
                if (SupportOpTypes.Contains(nullableTypeInfo.UnderlyingType))
                {// user op 
                    return CompareFunc(Expression.Property(p, propInfo), Expression.Convert(Expression.Constant(constValue), propInfo.PropertyType));
                }
                else
                {// use CompareTo
                    if (!typeof(IComparable<>).MakeGenericType(nullableTypeInfo.UnderlyingType).IsAssignableFrom(nullableTypeInfo.UnderlyingType))
                    {
                        throw new InvalidOperationException(string.Format("{0} 只能处理实现了IComparable<{1}>接口的类型", this.FilterType, nullableTypeInfo.UnderlyingType.FullName));
                    }
                    var left = propInfo.VisitExpression(p);
                    // reverse the expression to handle null value
                    var expression = ReverseCompareFunc(Expression.Call(
                                             Expression.Constant(constValue),
                                            nullableTypeInfo.UnderlyingType.GetMethod("CompareTo", new Type[] { nullableTypeInfo.UnderlyingType }),
                                           left), Expression.Constant(0));
                    return nullableTypeInfo.IsNullbale ?
                         Expression.AndAlso(
                                Expression.Property(Expression.Property(p, propInfo), nameof(Nullable<int>.HasValue)),
                                expression)
                            : expression;
                }
            }
        }
        class GreaterThanExpressionConverter : OpExpressionConverter
        {
            protected override CompareFunc CompareFunc => Expression.GreaterThan;
            protected override CompareFunc ReverseCompareFunc => Expression.LessThan;
        }
        class GreaterThanOrEqualExpressionConverter : OpExpressionConverter
        {
            protected override CompareFunc CompareFunc => Expression.GreaterThanOrEqual;
            protected override CompareFunc ReverseCompareFunc => Expression.LessThanOrEqual;
        }
        class LessThanExpressionConverter : OpExpressionConverter
        {
            protected override CompareFunc CompareFunc => Expression.LessThan;
            protected override CompareFunc ReverseCompareFunc => Expression.GreaterThan;
        }
        class LessThanOrEqualExpressionConverter : OpExpressionConverter
        {
            protected override CompareFunc CompareFunc => Expression.LessThanOrEqual;
            protected override CompareFunc ReverseCompareFunc => Expression.GreaterThanOrEqual;
        }
        class BetweenExpressionConverter : ExpressionConverter
        {
            public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
            {
                if (value == null) throw new InvalidOperationException(string.Format("{0} 无法处理null值", FilterType.Between));
                if (!(value is Array)) throw new InvalidOperationException(string.Format("{0} 值必须为数组", FilterType.Between));
                Array arr = value as Array;
                if (arr.Rank != 1 && arr.Length != 2) throw new InvalidOperationException(string.Format("{0} 值必须为长度为2的一维数组", FilterType.Between));
                var firstvalue = arr.GetValue(arr.GetLowerBound(0));
                var lastvalue = arr.GetValue(arr.GetLowerBound(0) + 1);
                if (firstvalue == null) throw new InvalidOperationException(string.Format("{0}的起始值不能为null", FilterType.Between));
                if (lastvalue == null) throw new InvalidOperationException(string.Format("{0}的结束值不能为null", FilterType.Between));
                var isnullabletype = propInfo.PropertyType.IsNullableType();
                var ptype = isnullabletype ? Nullable.GetUnderlyingType(propInfo.PropertyType) : propInfo.PropertyType;
                if (!typeof(IComparable<>).MakeGenericType(ptype).IsAssignableFrom(ptype))
                {
                    throw new InvalidOperationException(string.Format("{0} 只能处理实现了IComparable<{1}>接口的类型", FilterType.Between, ptype.FullName));
                }
                var propExpression = Expression.Property(p, propInfo);
                if (isnullabletype)
                    propExpression = Expression.Property(propExpression, "Value");
                var left = Expression.GreaterThanOrEqual(Expression.Call(
                                         propExpression,
                                         ptype.GetMethod("CompareTo", new Type[] { ptype }),
                                         Expression.Constant(this.ChangeType(firstvalue, ptype))), Expression.Constant(0));
                var right = Expression.LessThanOrEqual(Expression.Call(
                                         propExpression,
                                         ptype.GetMethod("CompareTo", new Type[] { ptype }),
                                         Expression.Constant(this.ChangeType(lastvalue, ptype))), Expression.Constant(0));
                return Expression.AndAlso(left, right);
            }
        }
        class NotBetweenExpressionConverter : BetweenExpressionConverter
        {
            public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
            {
                return Expression.Not(base.ConvertValue(p, propInfo, value));
            }
        }
        class InExpressionConverter : ExpressionConverter
        {
            public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
            {
                if (value == null) throw new InvalidOperationException(string.Format("{0} 无法处理null值", FilterType));
                IEnumerable arr = value as IEnumerable;
                if (arr == null) throw new InvalidOperationException(string.Format("{0} 值必须为可枚举类型", FilterType));
                var isnullabletype = propInfo.PropertyType.IsNullableType();
                var ptype = isnullabletype ? Nullable.GetUnderlyingType(propInfo.PropertyType) : propInfo.PropertyType;
                var lst = Activator.CreateInstance(typeof(List<>).MakeGenericType(propInfo.PropertyType)) as IList;
                foreach (var obj in arr)
                {
                    if (obj == null)
                    {
                        if (isnullabletype == false)
                        {
                            throw new InvalidCastException(string.Format("无法将null对象转换为{0}类型", propInfo.PropertyType.FullName));
                        }
                        else
                        {
                            lst.Add(null);
                        }
                    }
                    else
                    {
                        lst.Add(this.ChangeType(obj, ptype));
                    }
                }
                if (lst.Count == 0) return Expression.Constant(false);
                var methods = lst.GetType().GetMethod("Contains", new Type[] { propInfo.PropertyType });
                return Expression.Call(Expression.Constant(lst), methods, Expression.Property(p, propInfo));
                //return null;
            }
        }
        class NotInExpressionConverter : InExpressionConverter
        {
            public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
            {
                return Expression.Not(base.ConvertValue(p, propInfo, value));
            }
        }
    }
}
