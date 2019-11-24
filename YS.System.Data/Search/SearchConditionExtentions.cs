using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace System.Data
{

    public static class SearchConditionExtentions
    {
        public static IQueryable<T> WhereCondition<T>(this IQueryable<T> source, SearchCondition searchCondition)
        {
            if (searchCondition == null) return source;
           
            return source.Where(searchCondition.CreatePredicate<T>());
        }
        public static IQueryable<T> WhereCondition<T>(this IQueryable<T> source, params SearchCondition[] searchConditions)
        {
            if (searchConditions == null||searchConditions.Length==0) return source;
            foreach (var item in searchConditions)
            {
                source = WhereCondition(source, item);
            }
            return source;
        }
        public static Expression<Func<T, bool>> CreatePredicate<T>(this SearchCondition searchCondition)
        {
            if (searchCondition == null) return null;
            var p = Expression.Parameter(typeof(T), "p");
            var res = FromContidtionInternal(typeof(T), searchCondition, p);
            return Expression.Lambda<Func<T, bool>>(res, p);
        }
        private static Expression FromOrConditionInternal(Type entityType, SearchCondition orCondition, ParameterExpression p)
        {
            if (orCondition == null) throw new ArgumentNullException("orCondition");
            Expression current = Expression.Constant(false);
            foreach (SearchCondition item in orCondition.Items)
            {
                var next = FromContidtionInternal(entityType, item, p);
                current = Expression.OrElse(current, next);
            }
            return current;
        }
        private static Expression FromAndConditionInternal(Type entityType, SearchCondition andCondition, ParameterExpression p)
        {
            if (andCondition == null) throw new ArgumentNullException("andCondition");
            Expression current = Expression.Constant(true);
            foreach (SearchCondition item in andCondition.Items)
            {
                var next = FromContidtionInternal(entityType, item, p);

                current = Expression.AndAlso(current, next);
            }
            return current;
        }
        private static Expression FromItemConditionItemInternal(Type entityType, SearchCondition searchItem, ParameterExpression p)
        {
            if (searchItem == null) throw new ArgumentNullException("searchItem");
            if (string.IsNullOrEmpty(searchItem.FieldName)) throw new ArgumentException("FieldName必须填充");
            var paths = searchItem.FieldName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
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


            var val = searchItem.Value;
            if (val == DBNull.Value) val = null;//忽略dbnull.value
            var converter = GetConvertBySearchType(searchItem.SearchType);
            return converter.ConvertValue(exp, pinfo, val);
        }
        private static Expression FromContidtionInternal(Type entityType, SearchCondition searchCondition, ParameterExpression p)
        {
            if (searchCondition.OpType== OpType.SingleItem)
            {
                return FromItemConditionItemInternal(entityType, searchCondition, p);
            }
            else if (searchCondition.OpType== OpType.AndItems)
            {
                return FromAndConditionInternal(entityType, searchCondition, p);
            }
            else if (searchCondition.OpType == OpType.OrItems)
            {
                return FromOrConditionInternal(entityType, searchCondition, p);
            }
            else
            {
                throw new ArgumentException("unknow searchCondition");
            }
        }
        private static ExpressionConverter GetConvertBySearchType(SearchType searchType)
        {
            switch (searchType)
            {
                case SearchType.Equals:
                    return new EqualExpressionConverter();
                case SearchType.NotEquals:
                    return new NotEqualExpressionConverter();
                case SearchType.GreaterThan:
                    return new GreaterThanExpressionConverter();
                case SearchType.LessThanOrEqual:
                    return new LessThanOrEqualExpressionConverter();
                case SearchType.LessThan:
                    return new LessThanExpressionConverter();
                case SearchType.GreaterThanOrEqual:
                    return new GreaterThanOrEqualExpressionConverter();
                case SearchType.Between:
                    return new BetweenExpressionConverter();
                case SearchType.NotBetween:
                    return new NotBetweenExpressionConverter();
                case SearchType.In:
                    return new InExpressionConverter();
                case SearchType.NotIn:
                    return new NotInExpressionConverter();
                case SearchType.StartsWith:
                    return new StartWithExpressionConverter();
                case SearchType.NotStartsWith:
                    return new NotStartWithExpressionConverter();
                case SearchType.Contains:
                    return new ContainsExpressionConverter();
                case SearchType.NotContains:
                    return new NotContainsExpressionConverter();
                case SearchType.EndsWith:
                    return new EndWidhExpressionConverter();
                case SearchType.NotEndsWith:
                    return new NotEndWithExpressionConverter();
                default:
                    throw new ArgumentException(string.Format("无效的类型{0}", searchType));
            }
        }
    }

    abstract class ExpressionConverter
    {
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
                return Convert.ChangeType(value, changeType);
            }

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
                    throw new InvalidCastException(string.Format("无法将null对象转换为{0}类型", propInfo.PropertyType.FullName));
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
    class ContainsExpressionConverter : ExpressionConverter
    {
        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
        {

            if (value == null) throw new InvalidOperationException(string.Format("{0} 无法处理null值", SearchType.Contains));
            if (propInfo.PropertyType != typeof(string)) throw new InvalidOperationException(string.Format("{0} 只适用于string类型", SearchType.Contains));
            string val = Convert.ToString(value);
            var left = Expression.Property(p, propInfo);
            return Expression.Call(
                                     left,
                                     typeof(string).GetMethod("Contains", new Type[] { typeof(string) }),
                                     Expression.Constant(val)
                                     );
        }
    }
    class NotContainsExpressionConverter : ExpressionConverter
    {
        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
        {
            if (value == null) throw new InvalidOperationException(string.Format("{0} 无法处理null值", SearchType.NotContains));
            if (propInfo.PropertyType != typeof(string)) throw new InvalidOperationException(string.Format("{0} 只适用于string类型", SearchType.NotContains));
            string val = Convert.ToString(value);
            var left = Expression.Property(p, propInfo);
            return Expression.Not(
                         Expression.Call(
                                     left,
                                     typeof(string).GetMethod("Contains", new Type[] { typeof(string) }),
                                     Expression.Constant(val)
                                     ));
        }
    }
    class StartWithExpressionConverter : ExpressionConverter
    {
        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
        {
            if (value == null) throw new InvalidOperationException(string.Format("{0} 无法处理null值", SearchType.StartsWith));
            if (propInfo.PropertyType != typeof(string)) throw new InvalidOperationException(string.Format("{0} 只适用于string类型", SearchType.StartsWith));
            string val = Convert.ToString(value);
            var left = Expression.Property(p, propInfo);
            return Expression.Call(
                                     left,
                                     typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }),
                                     Expression.Constant(val)
                                     );
        }

    }
    class NotStartWithExpressionConverter : ExpressionConverter
    {
        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
        {
            if (value == null) throw new InvalidOperationException(string.Format("{0} 无法处理null值", SearchType.NotStartsWith));
            if (propInfo.PropertyType != typeof(string)) throw new InvalidOperationException(string.Format("{0} 只适用于string类型", SearchType.NotStartsWith));
            string val = Convert.ToString(value);
            var left = Expression.Property(p, propInfo);
            return
                Expression.Not(
                         Expression.Call(
                                     left,
                                     typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }),
                                     Expression.Constant(val)
                                     ));
        }

    }
    class EndWidhExpressionConverter : ExpressionConverter
    {
        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
        {
            if (value == null) throw new InvalidOperationException(string.Format("{0} 无法处理null值", SearchType.EndsWith));
            if (propInfo.PropertyType != typeof(string)) throw new InvalidOperationException(string.Format("{0} 只适用于string类型", SearchType.EndsWith));
            string val = Convert.ToString(value);
            var left = Expression.Property(p, propInfo);
            return Expression.Call(left,
                                     typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }),
                                     Expression.Constant(val)
                                     );
        }
    }
    class NotEndWithExpressionConverter : ExpressionConverter
    {
        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
        {
            if (value == null) throw new InvalidOperationException(string.Format("{0} 无法处理null值", SearchType.NotEndsWith));
            if (propInfo.PropertyType != typeof(string)) throw new InvalidOperationException(string.Format("{0} 只适用于string类型", SearchType.NotEndsWith));
            string val = Convert.ToString(value);
            var left = Expression.Property(p, propInfo);
            return Expression.Not(
                         Expression.Call(
                                     left,
                                     typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }),
                                     Expression.Constant(val)
                                     ));
        }

    }
    class GreaterThanExpressionConverter : ExpressionConverter
    {
        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
        {
            if (value == null) throw new InvalidOperationException(string.Format("{0} 无法处理null值", SearchType.GreaterThan));
            var isnullabletype = propInfo.PropertyType.IsNullableType();
            var ptype = isnullabletype ? Nullable.GetUnderlyingType(propInfo.PropertyType) : propInfo.PropertyType;
            if (!typeof(IComparable<>).MakeGenericType(ptype).IsAssignableFrom(ptype))
            {
                throw new InvalidOperationException(string.Format("{0} 只能处理实现了IComparable<{1}>接口的类型", SearchType.GreaterThan, ptype.FullName));
            }
            object val = this.ChangeType(value, ptype);
            var left = Expression.Property(p, propInfo);
            if (isnullabletype)
                left = Expression.Property(left, "Value");
            return Expression.GreaterThan(Expression.Call(
                                     left,
                                     ptype.GetMethod("CompareTo", new Type[] { ptype }),
                                     Expression.Constant(val)), Expression.Constant(0));

        }
    }
    class GreaterThanOrEqualExpressionConverter : ExpressionConverter
    {
        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
        {
            if (value == null) throw new InvalidOperationException(string.Format("{0} 无法处理null值", SearchType.GreaterThan));
            var isnullabletype = propInfo.PropertyType.IsNullableType();
            var ptype = isnullabletype ? Nullable.GetUnderlyingType(propInfo.PropertyType) : propInfo.PropertyType;
            if (!typeof(IComparable<>).MakeGenericType(ptype).IsAssignableFrom(ptype))
            {
                throw new InvalidOperationException(string.Format("{0} 只能处理实现了IComparable<{1}>接口的类型", SearchType.GreaterThan, ptype.FullName));
            }
            object val = this.ChangeType(value, ptype);
            var left = Expression.Property(p, propInfo);
            if (isnullabletype)
                left = Expression.Property(left, "Value");
            return Expression.GreaterThanOrEqual(Expression.Call(
                                     left,
                                     ptype.GetMethod("CompareTo", new Type[] { ptype }),
                                     Expression.Constant(val)), Expression.Constant(0));

        }
    }
    class LessThanExpressionConverter : ExpressionConverter
    {
        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
        {
            if (value == null) throw new InvalidOperationException(string.Format("{0} 无法处理null值", SearchType.GreaterThan));
            var isnullabletype = propInfo.PropertyType.IsNullableType();
            var ptype = isnullabletype ? Nullable.GetUnderlyingType(propInfo.PropertyType) : propInfo.PropertyType;
            if (!typeof(IComparable<>).MakeGenericType(ptype).IsAssignableFrom(ptype))
            {
                throw new InvalidOperationException(string.Format("{0} 只能处理实现了IComparable<{1}>接口的类型", SearchType.GreaterThan, ptype.FullName));
            }
            object val = this.ChangeType(value, ptype);
            var left = Expression.Property(p, propInfo);
            if (isnullabletype)
                left = Expression.Property(left, "Value");
            return Expression.LessThan(Expression.Call(
                                     left,
                                     ptype.GetMethod("CompareTo", new Type[] { ptype }),
                                     Expression.Constant(val)), Expression.Constant(0));
        }
    }
    class LessThanOrEqualExpressionConverter : ExpressionConverter
    {
        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
        {
            if (value == null) throw new InvalidOperationException(string.Format("{0} 无法处理null值", SearchType.GreaterThan));
            var isnullabletype = propInfo.PropertyType.IsNullableType();
            var ptype = isnullabletype ? Nullable.GetUnderlyingType(propInfo.PropertyType) : propInfo.PropertyType;
            if (!typeof(IComparable<>).MakeGenericType(ptype).IsAssignableFrom(ptype))
            {
                throw new InvalidOperationException(string.Format("{0} 只能处理实现了IComparable<{1}>接口的类型", SearchType.GreaterThan, ptype.FullName));
            }
            object val = this.ChangeType(value, ptype);
            var left = Expression.Property(p, propInfo);
            if (isnullabletype)
                left = Expression.Property(left, "Value");
            return Expression.LessThanOrEqual(Expression.Call(
                                     left,
                                     ptype.GetMethod("CompareTo", new Type[] { ptype }),
                                     Expression.Constant(val)), Expression.Constant(0));
        }
    }
    class BetweenExpressionConverter : ExpressionConverter
    {
        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
        {
            if (value == null) throw new InvalidOperationException(string.Format("{0} 无法处理null值", SearchType.Between));
            if (!(value is Array)) throw new InvalidOperationException(string.Format("{0} 值必须为数组", SearchType.Between));
            Array arr = value as Array;
            if (arr.Rank != 1 && arr.Length != 2) throw new InvalidOperationException(string.Format("{0} 值必须为长度为2的一维数组", SearchType.Between));
            var firstvalue = arr.GetValue(arr.GetLowerBound(0));
            var lastvalue = arr.GetValue(arr.GetLowerBound(0) + 1);
            if (firstvalue == null) throw new InvalidOperationException(string.Format("{0}的起始值不能为null", SearchType.Between));
            if (lastvalue == null) throw new InvalidOperationException(string.Format("{0}的结束值不能为null", SearchType.Between));
            var isnullabletype = propInfo.PropertyType.IsNullableType();
            var ptype = isnullabletype ? Nullable.GetUnderlyingType(propInfo.PropertyType) : propInfo.PropertyType;
            if (!typeof(IComparable<>).MakeGenericType(ptype).IsAssignableFrom(ptype))
            {
                throw new InvalidOperationException(string.Format("{0} 只能处理实现了IComparable<{1}>接口的类型", SearchType.Between, ptype.FullName));
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
    class NotBetweenExpressionConverter : ExpressionConverter
    {

        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
        {
            if (value == null) throw new InvalidOperationException(string.Format("{0} 无法处理null值", SearchType.NotBetween));
            if (!(value is Array)) throw new InvalidOperationException(string.Format("{0} 值必须为数组", SearchType.NotBetween));
            Array arr = value as Array;
            if (arr.Rank != 1 && arr.Length != 2) throw new InvalidOperationException(string.Format("{0} 值必须为长度为2的一维数组", SearchType.NotBetween));
            var firstvalue = arr.GetValue(arr.GetLowerBound(0));
            var lastvalue = arr.GetValue(arr.GetLowerBound(0) + 1);
            if (firstvalue == null) throw new InvalidOperationException(string.Format("{0}的起始值不能为null", SearchType.NotBetween));
            if (lastvalue == null) throw new InvalidOperationException(string.Format("{0}的结束值不能为null", SearchType.NotBetween));
            var isnullabletype = propInfo.PropertyType.IsNullableType();
            var ptype = isnullabletype ? Nullable.GetUnderlyingType(propInfo.PropertyType) : propInfo.PropertyType;
            if (!typeof(IComparable<>).MakeGenericType(ptype).IsAssignableFrom(ptype))
            {
                throw new InvalidOperationException(string.Format("{0} 只能处理实现了IComparable<{1}>接口的类型", SearchType.Between, ptype.FullName));
            }

            var propExpression = Expression.Property(p, propInfo);
            if (isnullabletype)
                propExpression = Expression.Property(propExpression, "Value");

            var left = Expression.LessThan(Expression.Call(
                                     propExpression,
                                     ptype.GetMethod("CompareTo", new Type[] { ptype }),
                                     Expression.Constant(this.ChangeType(firstvalue, ptype))), Expression.Constant(0));

            var right = Expression.GreaterThan(Expression.Call(
                                     propExpression,
                                     ptype.GetMethod("CompareTo", new Type[] { ptype }),
                                     Expression.Constant(this.ChangeType(lastvalue, ptype))), Expression.Constant(0));

            return Expression.OrElse(left, right);
        }
    }
    class InExpressionConverter : ExpressionConverter
    {
        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
        {
            if (value == null) throw new InvalidOperationException(string.Format("{0} 无法处理null值", SearchType.In));
            //if (!(value is Array)) throw new InvalidOperationException(string.Format("{0} 值必须为数组", SearchType.In));
            //Array arr = value as Array;
            //if (arr.Rank != 1) throw new InvalidOperationException(string.Format("{0} 值必须为一维数组", SearchType.In));
            //if (arr.Length == 0) return Expression.Constant(false);
            IEnumerable arr = value as IEnumerable;
            if (arr == null) throw new InvalidOperationException(string.Format("{0} 值必须为可枚举类型", SearchType.NotIn));

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
    class NotInExpressionConverter : ExpressionConverter
    {

        public override Expression ConvertValue(Expression p, PropertyInfo propInfo, object value)
        {
            if (value == null) throw new InvalidOperationException(string.Format("{0} 无法处理null值", SearchType.NotIn));
            IEnumerable arr = value as IEnumerable;
            if (arr == null) throw new InvalidOperationException(string.Format("{0} 值必须为可枚举类型", SearchType.NotIn));
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
            if (lst.Count == 0) return Expression.Constant(true);
            var methods = lst.GetType().GetMethod("Contains", new Type[] { propInfo.PropertyType });
            return Expression.Not(Expression.Call(Expression.Constant(lst), methods, Expression.Property(p, propInfo)));
        }
    }
}
