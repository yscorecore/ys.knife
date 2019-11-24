using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace System.Data
{
    public sealed class Exp<T>
    {
        public static OrderItem CreateOrder(Expression<Func<T, object>> keySelector, OrderType orderType)
        {
            if (orderType == OrderType.ASC)
            {
                return CreateOrderAsc(keySelector);
            }
            else
            {
                return CreateOrderDesc(keySelector);
            }
        }

        public static OrderItem CreateOrderAsc(Expression<Func<T, object>> keySelector)
        {
            var exp = keySelector.Body;
            if (keySelector.Body.NodeType == ExpressionType.Convert)
            {
                exp = (keySelector.Body as System.Linq.Expressions.UnaryExpression).Operand;
            }
            if (exp.NodeType == ExpressionType.MemberAccess)
            {
                System.Linq.Expressions.MemberExpression me = exp as System.Linq.Expressions.MemberExpression;
                return new OrderItem() { FieldName = GetFieldNameFromMember(me), OrderType = OrderType.ASC };
            }
            else
            {
                throw new ArgumentException("NodeType必须为ExpressionType.MemberAccess类型");
            }
        }

        public static OrderItem CreateOrderDesc(Expression<Func<T, object>> keySelector)
        {
            var exp = keySelector.Body;
            if (keySelector.Body.NodeType == ExpressionType.Convert)
            {
                exp = (keySelector.Body as System.Linq.Expressions.UnaryExpression).Operand;
            }
            if (exp.NodeType == ExpressionType.MemberAccess)
            {
                System.Linq.Expressions.MemberExpression me = exp as System.Linq.Expressions.MemberExpression;
                return new OrderItem() { FieldName = GetFieldNameFromMember(me), OrderType = OrderType.DESC };
            }
            else
            {
                throw new ArgumentException("NodeType必须为ExpressionType.MemberAccess类型");
            }
        }

        public static SearchCondition CreateSearch(Expression<Func<T, bool>> predicate)
        {
            return CreateFromExpression(predicate.Body);
        }

        public static string FieldName(Expression<Func<T, object>> keySelector)
        {
            var exp = keySelector.Body;
            if (keySelector.Body.NodeType == ExpressionType.Convert)
            {
                exp = (keySelector.Body as System.Linq.Expressions.UnaryExpression).Operand;
            }
            if (exp.NodeType == ExpressionType.MemberAccess)
            {
                System.Linq.Expressions.MemberExpression me = exp as System.Linq.Expressions.MemberExpression;
                return GetFieldNameFromMemberIgNoreFirst(me);
            }
            else
            {
                throw new ArgumentException("NodeType必须为ExpressionType.MemberAccess类型");
            }
        }
        private static string GetFieldNameFromMemberIgNoreFirst(MemberExpression me)
        {
            if (me == null) return string.Empty;

            if (me.Member.Name == "Value" && me.Expression != null && me.Expression.NodeType == ExpressionType.MemberAccess && me.Member.DeclaringType.GetGenericTypeDefinition() == (typeof(Nullable<>)))
            {
                me = me.Expression as MemberExpression;
            }

            List<string> paths = new List<string>();
            while (true)
            {
                if (me.Expression.NodeType == ExpressionType.Parameter)
                {
                    paths.Insert(0, GetMemberFieldName(me.Member));
                    break;
                }
                else if (me.Expression.NodeType == ExpressionType.MemberAccess)
                {
                    paths.Insert(0, GetMemberFieldName(me.Member));
                    me = me.Expression as MemberExpression;
                }
                else if (me.Expression.NodeType == ExpressionType.Call)
                {
                    var callExp = (me.Expression as MethodCallExpression);

                    if (callExp.Method.DeclaringType == typeof(Enumerable) && callExp.Method.Name == "First" || callExp.Method.Name == "FirstOrDefault")
                    {
                        paths.Insert(0, GetMemberFieldName(me.Member));
                        me = callExp.Arguments[0] as MemberExpression;
                    }
                    else
                    {
                        throw new ArgumentException("无法处理的类型");

                    }

                }
                else
                {
                    throw new ArgumentException("无法处理的类型");
                }
            }

            return paths.Join(".");
        }
        public static string[] FieldNames(params Expression<Func<T, object>>[] keySelectors)
        {
            string[] res = new string[keySelectors == null ? 0 : keySelectors.Length];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = FieldName(keySelectors[i]);
            }
            return res;
        }
        private static SearchCondition CreateFromExpression(Expression body)
        {
            if (body is BinaryExpression)
            {
                return CreateFromBinary(body as BinaryExpression);
            }
            else if (body is UnaryExpression)
            {
                if (body.NodeType == ExpressionType.Not)
                {
                    var sc = CreateFromExpression((body as UnaryExpression).Operand);
                    return sc.Not();
                }
                else
                {
                    throw new ArgumentException(string.Format("不支持的一元运算符号{0}", body.NodeType));
                }
            }
            else if (body is MethodCallExpression)
            {
                return CreateItemFromBinaryExpression(Expression.Equal(body, Expression.Constant(true)));
            }
            else if (body is MemberExpression)
            {
                return CreateItemFromBinaryExpression(Expression.Equal(body, Expression.Constant(true)));
            }
            else
            {
                throw new ArgumentException(string.Format("无法将表达式{0}转为类型{1}", body, typeof(SearchCondition).FullName));
            }
        }
        private static SearchCondition CreateFromBinary(BinaryExpression bin)
        {
            if (bin.NodeType == ExpressionType.AndAlso)
            {
                SearchCondition and = new SearchCondition() { OpType = OpType.AndItems };
                and = and.AndAlso(CreateFromExpression(bin.Left));
                and = and.AndAlso(CreateFromExpression(bin.Right));
                return and;
            }
            else if (bin.NodeType == ExpressionType.OrElse)
            {
                SearchCondition or = new SearchCondition() { OpType = OpType.OrItems };
                or = or.OrElse(CreateFromExpression(bin.Left));
                or = or.OrElse(CreateFromExpression(bin.Right));
                return or;
            }
            else
            {
                return CreateItemFromBinaryExpression(bin);
            }
        }
        private static SearchCondition CreateItemFromBinaryExpression(BinaryExpression bin)
        {
            var isleftFieldAccess = IsFieldVisitExpression(bin.Left);
            var isrightFieldAccess = IsFieldVisitExpression(bin.Right);
            if (isleftFieldAccess && isrightFieldAccess)
            {
                throw new ArgumentException(string.Format("表达式{0}左侧和右侧不能同时引用参数", bin));
            }
            else if ((isleftFieldAccess || isrightFieldAccess) == false)
            {
                throw new ArgumentException(string.Format("表达式{0}左侧和右侧至少需要有一侧引用参数", bin));
            }
            else if (isrightFieldAccess)
            {
                throw new ArgumentException(string.Format("参数必须在表达式{0}的左侧", bin));
            }
            else
            {
                var searchType = GetSearchTypeByNodeType(bin.NodeType);
                return CreateItem(bin.Left, searchType, Expression.Lambda(bin.Right).Compile().DynamicInvoke());
            }
        }
        private static bool IsFieldVisitExpression(Expression exp)
        {
            if (exp is UnaryExpression)
            {
                var unary = exp as UnaryExpression;
                if (unary.Operand == null) return false;
                return IsFieldVisitExpression(unary.Operand);
            }
            else if (exp is MethodCallExpression)
            {
                var call = exp as MethodCallExpression;
                var res = false;
                if (call.Object != null)
                {
                    res = IsFieldVisitExpression(call.Object);
                }
                if (res == false && call.Arguments != null)
                {
                    foreach (Expression arg in call.Arguments)
                    {
                        res = IsFieldVisitExpression(arg);
                        if (res) break;
                    }
                }
                return res;
            }
            else if (exp is MemberExpression)
            {
                var me = exp as MemberExpression;
                if (me.Expression == null) return false;
                if (me.Expression.NodeType == ExpressionType.Parameter)
                {
                    return true;
                }
                return IsFieldVisitExpression(me.Expression);
            }
            else
            {
                return false;
            }
        }
        private static SearchCondition CreateItem(Expression exp, SearchType searchType, object value)
        {
            if (exp.NodeType == ExpressionType.Call)
            {
                return CreateItemFromCall(exp as MethodCallExpression, searchType, value);
            }
            else if (exp.NodeType == ExpressionType.MemberAccess)
            {
                return CreateItemFromMemberAccess(exp as MemberExpression, searchType, value);
            }
            else if (exp.NodeType == ExpressionType.Convert)
            {
                return CreateItemFromConvert(exp as UnaryExpression, searchType, value);
            }
            else
            {
                throw new ArgumentException(string.Format("无法转换表达式{0}", exp));
            }

        }
        private static SearchCondition CreateItemFromConvert(UnaryExpression me, SearchType searchType, object val)
        {

            return CreateItem(me.Operand, searchType, val);
        }
        private static SearchCondition CreateItemFromMemberAccess(MemberExpression me, SearchType searchType, object val)
        {
            string name = GetFieldNameFromMember(me);
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(string.Format("无法从转换表达式{0}中推断出字段名称", me));
            }
            return new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = name, SearchType = searchType, Value = val };

        }
        private static SearchCondition CreateItemFromCall(MethodCallExpression mce, SearchType searchType, object val)
        {
            if (mce.Method.Name == "Equals")
            {
                return CreateItemFromEqualsCall(mce, searchType, val);
            }
            else if (mce.Method.Name == "StartsWith")
            {
                return CreateItemFromStartWithCall(mce, searchType, val);
            }
            else if (mce.Method.Name == "EndsWith")
            {
                return CreateItemFromEndWithCall(mce, searchType, val);
            }
            else if (mce.Method.Name == "Contains")
            {
                return CreateItemFromContainsCall(mce, searchType, val);
            }
            else if (mce.Method.Name == "CompareTo")
            {
                return CreateItemFromCompareToCall(mce, searchType, val);
            }
            else
            {
                throw new ArgumentException(string.Format("无法转换表达式{0}", mce));
            }
        }
        private static SearchCondition CreateItemFromStartWithCall(MethodCallExpression mce, SearchType searchType, object value)
        {
            if (searchType == SearchType.Equals || searchType == SearchType.NotEquals)
            {
                bool val = Convert.ToBoolean(value);
                if (mce.Object != null && mce.Object is MemberExpression && mce.Arguments != null && mce.Arguments.Count == 1)
                {
                    string name = GetFieldNameFromMember(mce.Object as MemberExpression);
                    if (string.IsNullOrEmpty(name))
                    {
                        throw new ArgumentException(string.Format("无法从转换表达式{0}中推断出字段名称", mce));
                    }
                    object argvalue = Expression.Lambda(mce.Arguments[0]).Compile().DynamicInvoke();
                    if ((searchType == SearchType.Equals && val == true) || (searchType == SearchType.NotEquals) && (val == false))
                    {
                        return new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = name, Value = argvalue, SearchType = SearchType.StartsWith };
                    }
                    else
                    {
                        return new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = name, Value = argvalue, SearchType = SearchType.NotStartsWith };
                    }
                }
            }
            throw new ArgumentException(string.Format("无法转换表达式{0}", mce));

        }
        private static SearchCondition CreateItemFromEqualsCall(MethodCallExpression mce, SearchType searchType, object value)
        {
            if (searchType == SearchType.Equals || searchType == SearchType.NotEquals)
            {
                bool val = Convert.ToBoolean(value);
                if (mce.Object != null && mce.Object is MemberExpression && mce.Arguments != null && mce.Arguments.Count == 1)
                {
                    string name = GetFieldNameFromMember(mce.Object as MemberExpression);
                    if (string.IsNullOrEmpty(name))
                    {
                        throw new ArgumentException(string.Format("无法从转换表达式{0}中推断出字段名称", mce));
                    }
                    object argvalue = Expression.Lambda(mce.Arguments[0]).Compile().DynamicInvoke();
                    if ((searchType == SearchType.Equals && val == true) || (searchType == SearchType.NotEquals) && (val == false))
                    {
                        return new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = name, Value = argvalue, SearchType = SearchType.Equals };
                    }
                    else
                    {
                        return new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = name, Value = argvalue, SearchType = SearchType.NotEquals };
                    }
                }
            }
            throw new ArgumentException(string.Format("无法转换表达式{0}", mce));
        }

        private static SearchCondition CreateItemFromEndWithCall(MethodCallExpression mce, SearchType searchType, object value)
        {
            if (searchType == SearchType.Equals || searchType == SearchType.NotEquals)
            {
                bool val = Convert.ToBoolean(value);
                if (mce.Object != null && mce.Object is MemberExpression && mce.Arguments != null && mce.Arguments.Count == 1)
                {
                    string name = GetFieldNameFromMember(mce.Object as MemberExpression);
                    if (string.IsNullOrEmpty(name))
                    {
                        throw new ArgumentException(string.Format("无法从转换表达式{0}中推断出字段名称", mce));
                    }
                    object argvalue = Expression.Lambda(mce.Arguments[0]).Compile().DynamicInvoke();
                    if ((searchType == SearchType.Equals && val == true) || (searchType == SearchType.NotEquals) && (val == false))
                    {
                        return new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = name, Value = argvalue, SearchType = SearchType.EndsWith };
                    }
                    else
                    {
                        return new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = name, Value = argvalue, SearchType = SearchType.NotEndsWith };
                    }
                }
            }
            throw new ArgumentException(string.Format("无法转换表达式{0}", mce));

        }

        private static SearchCondition CreateItemFromCompareToCall(MethodCallExpression mce, SearchType searchType, object value)
        {
            int val = Convert.ToInt32(value);
            if (!new int[] { 0, 1, -1 }.Contains(val)) throw new ArgumentException(string.Format("表达式{0}比较的值必须为 0,1或-1", mce));
            object comvalue = null;
            if (mce.Object != null && mce.Object is MemberExpression && mce.Arguments != null && mce.Arguments.Count == 1)
            {
                string name = GetFieldNameFromMember(mce.Object as MemberExpression);
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException(string.Format("无法从转换表达式{0}中推断出字段名称", mce));
                }
                comvalue = Expression.Lambda(mce.Arguments[0]).Compile().DynamicInvoke();
                if (searchType == SearchType.Equals)
                {
                    if (val == 0)
                    {
                        return new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = name, SearchType = SearchType.Equals, Value = comvalue };
                    }
                    else if (val == 1)
                    {
                        return new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = name, SearchType = SearchType.GreaterThan, Value = comvalue };
                    }
                    else
                    {
                        return new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = name, SearchType = SearchType.LessThan, Value = comvalue };
                    }
                }
                else if (searchType == SearchType.NotEquals)
                {
                    if (val == 0)
                    {
                        return new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = name, SearchType = SearchType.NotEquals, Value = comvalue };
                    }
                    else if (val == 1)
                    {
                        return new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = name, SearchType = SearchType.LessThanOrEqual, Value = comvalue };
                    }
                    else
                    {
                        return new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = name, SearchType = SearchType.GreaterThanOrEqual, Value = comvalue };
                    }
                }
                else if (new SearchType[] { SearchType.LessThan, SearchType.GreaterThan, SearchType.LessThanOrEqual, SearchType.GreaterThanOrEqual }.Contains(searchType))
                {
                    if (val != 0) throw new ArgumentException(string.Format("表达式{0}比较结果的值必须和0做参照", mce));
                    return new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = name, SearchType = searchType, Value = comvalue };
                }
            }
            throw new ArgumentException(string.Format("无法转换表达式{0}", mce));
        }

        private static SearchCondition CreateItemFromContainsCall(MethodCallExpression mce, SearchType searchType, object value)
        {
            bool val = Convert.ToBoolean(value);
            if (searchType == SearchType.Equals || searchType == SearchType.NotEquals)
            {
                if (mce.Method.DeclaringType == typeof(string))//like
                {
                    if (mce.Object != null && mce.Object is MemberExpression && mce.Arguments != null && mce.Arguments.Count == 1)
                    {
                        string name = GetFieldNameFromMember(mce.Object as MemberExpression);
                        if (string.IsNullOrEmpty(name))
                        {
                            throw new ArgumentException(string.Format("无法从转换表达式{0}中推断出字段名称", mce));
                        }
                        object argvalue = Expression.Lambda(mce.Arguments[0]).Compile().DynamicInvoke();
                        if ((searchType == SearchType.Equals && val == true) || (searchType == SearchType.NotEquals) && (val == false))
                        {
                            return new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = name, Value = argvalue, SearchType = SearchType.Contains };
                        }
                        else
                        {
                            return new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = name, Value = argvalue, SearchType = SearchType.NotContains };
                        }
                    }
                }
                else //in ,not in
                {
                    if (mce.Arguments.Count == 2)
                    {
                        string name = GetFieldNameFromMember(mce.Arguments[1] as MemberExpression);
                        if (string.IsNullOrEmpty(name))
                        {
                            throw new ArgumentException(string.Format("无法从转换表达式{0}中推断出字段名称", mce));
                        }
                        object argvalue = Expression.Lambda(mce.Arguments[0]).Compile().DynamicInvoke();
                        if ((searchType == SearchType.Equals && val == true) || (searchType == SearchType.NotEquals) && (val == false))
                        {
                            return new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = name, Value = argvalue, SearchType = SearchType.In };
                        }
                        else
                        {
                            return new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = name, Value = argvalue, SearchType = SearchType.NotIn };
                        }
                    }
                    else if (mce.Object != null && mce.Arguments != null && mce.Arguments.Count == 1)
                    {

                        string name = GetFieldNameFromMember(mce.Arguments[0] as MemberExpression);
                        if (string.IsNullOrEmpty(name))
                        {
                            throw new ArgumentException(string.Format("无法从转换表达式{0}中推断出字段名称", mce));
                        }
                        object argvalue = Expression.Lambda(mce.Object).Compile().DynamicInvoke();
                        if ((searchType == SearchType.Equals && val == true) || (searchType == SearchType.NotEquals) && (val == false))
                        {
                            return new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = name, Value = argvalue, SearchType = SearchType.In };
                        }
                        else
                        {
                            return new SearchCondition() { OpType = Data.OpType.SingleItem, FieldName = name, Value = argvalue, SearchType = SearchType.NotIn };
                        }
                    }
                }
            }
            throw new ArgumentException(string.Format("无法转换表达式{0}", mce));
        }

        private static SearchType GetSearchTypeByNodeType(ExpressionType expType)
        {
            switch (expType)
            {
                case ExpressionType.Equal:
                    return SearchType.Equals;
                case ExpressionType.NotEqual:
                    return SearchType.NotEquals;
                case ExpressionType.GreaterThan:
                    return SearchType.GreaterThan;
                case ExpressionType.LessThan:
                    return SearchType.LessThan;
                case ExpressionType.LessThanOrEqual:
                    return SearchType.LessThanOrEqual;
                case ExpressionType.GreaterThanOrEqual:
                    return SearchType.GreaterThanOrEqual;
                default:
                    throw new ArgumentException(string.Format("无法识别的的ExpressionType {0}", expType));
            }
        }

        private static string GetFieldNameFromMember(MemberExpression me)
        {
            if (me == null) return string.Empty;
            if (IsNullableType(me) && me.Member.Name == "Value" && me.Expression != null && me.Expression.NodeType == ExpressionType.MemberAccess)//可为空类型
            {
                me = me.Expression as MemberExpression;
            }
            List<string> paths = new List<string>();

            while (true)
            {
                if (me.Expression.NodeType == ExpressionType.Parameter)
                {
                    paths.Insert(0, GetMemberFieldName(me.Member));
                    break;
                }
                else if (me.Expression.NodeType == ExpressionType.MemberAccess)
                {
                    paths.Insert(0, GetMemberFieldName(me.Member));
                    me = me.Expression as MemberExpression;
                }
                else
                {
                    //throw new ArgumentException("无法处理的类型");
                    break;
                }
            }

            return paths.Join(".");

            //if (me.Expression.NodeType == ExpressionType.Parameter)
            //{
            //    return me.Member.Name;
            //}
            //else if (me.Expression.NodeType == ExpressionType.MemberAccess)
            //{

            //}
            //else
            //{
            //    return string.Empty;
            //}
        }

        private static string GetMemberFieldName(MemberInfo m)
        {
            var field = m.GetCustomAttribute<FieldNameAttribute>();
            if (field != null && !string.IsNullOrEmpty(field.FieldName))
            {
                return field.FieldName;
            }
            return m.Name;
        }

        private static bool IsNullableType(MemberExpression me)
        {
            return ((me.Member is PropertyInfo) && (me.Member as PropertyInfo).PropertyType.IsNullableType())
                || ((me.Member is FieldInfo) && (me.Member as FieldInfo).FieldType.IsNullableType());
        }
    }
}
