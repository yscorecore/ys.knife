using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using YS.Knife.Data.Filter.Operators;
using YS.Knife.Data.Mappers;
using YS.Knife.Data.Query;
using YS.Knife.Data.Query.Expressions;
using YS.Knife.Data.Query.Functions;

namespace YS.Knife.Data.Query.Expressions
{
}

namespace YS.Knife.Data.Filter
{
    [Obsolete]
    public class FilterInfoExpressionBuilder
    {
        internal static FilterInfoExpressionBuilder Default = new FilterInfoExpressionBuilder();

        public Expression<Func<TSource, bool>> CreateFilterLambdaExpression<TSource, TTarget>(
           ObjectMapper<TSource, TTarget> mapper, FilterInfo targetFilter)
           where TSource : class
           where TTarget : class, new()
        {
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
            var memberExpressionProvider = IMemberVisitor.GetMapperVisitor(mapper);
            return CreateFilterLambdaExpression<TSource>(targetFilter, memberExpressionProvider);
        }

        public Expression<Func<T, bool>> CreateFilterLambdaExpression<T>(
            FilterInfo filter)
        {
            var memberExpressionProvider = IMemberVisitor.GetObjectVisitor(typeof(T));
            return CreateFilterLambdaExpression<T>(filter, memberExpressionProvider);

        }
        public Expression<Func<T, bool>> CreateFilterLambdaExpression<T>(FilterInfo filterInfo, IMemberVisitor memberVisitor)
        {
            var p = Expression.Parameter(typeof(T), "p");
            var expression = CreateFilterExpression(p, filterInfo, memberVisitor);
            return Expression.Lambda<Func<T, bool>>(expression, p);
        }
        public LambdaExpression CreateFilterLambdaExpression(Type objectType, FilterInfo filterInfo, IMemberVisitor memberVisitor)
        {
            var p = Expression.Parameter(objectType, "p");
            var expression = CreateFilterExpression(p, filterInfo, memberVisitor);
            return Expression.Lambda(typeof(Func<,>).MakeGenericType(objectType, typeof(bool)), expression, p);

        }
        internal Expression CreateFilterExpression(ParameterExpression p, FilterInfo filterInfo, IMemberVisitor memberVisitor)
        {
            if (filterInfo == null)
            {
                return Expression.Constant(true);
            }
            else
            {
                return CreateCombinGroupsFilterExpression(p, filterInfo, memberVisitor);
            }
        }
        private Expression CreateCombinGroupsFilterExpression(ParameterExpression p, FilterInfo filterInfo, IMemberVisitor memberVisitor)
        {
            return filterInfo.OpType switch
            {
                CombinSymbol.AndItems => CreateAndConditionFilterExpression(p, filterInfo, memberVisitor),
                CombinSymbol.OrItems => CreateOrConditionFilterExpression(p, filterInfo, memberVisitor),
                _ => CreateSingleItemFilterExpression(p, filterInfo, memberVisitor)
            };
        }
        private Expression CreateOrConditionFilterExpression(ParameterExpression p, FilterInfo orGroupFilterInfo, IMemberVisitor memberVisitor)
        {
            Expression current = Expression.Constant(false);
            foreach (FilterInfo item in orGroupFilterInfo.Items.TrimNotNull())
            {
                var next = CreateCombinGroupsFilterExpression(p, item, memberVisitor);
                current = Expression.OrElse(current, next);
            }
            return current;
        }
        private Expression CreateAndConditionFilterExpression(ParameterExpression p, FilterInfo andGroupFilterInfo, IMemberVisitor memberVisitor)
        {

            Expression current = Expression.Constant(true);
            foreach (FilterInfo item in andGroupFilterInfo.Items.TrimNotNull())
            {
                var next = CreateCombinGroupsFilterExpression(p, item, memberVisitor);
                current = Expression.AndAlso(current, next);
            }
            return current;
        }
        private Expression CreateSingleItemFilterExpression(ParameterExpression p, FilterInfo singleItemFilter
          , IMemberVisitor memberVisitor)
        {
            /*** 
            a.b.c = null    >  a==null || a.b==null || a.b.c == null
            a.b.c ="abc"   >  a!=null && a.b!=null && a.b.c = "abc"
            a.b.c != null    > a!=null && a.b!=null && a.b.c !=null
            a.b.c != "abc"  > a==null || a.b==null || a.b.c

            a.b.c ct "abc"  > a!=null && b!=null && a.b.c!=null && a.b.c ct "abc" 
            a.b.c nct "abc" > a==null || b==null || c==null || a.b.c nct "abc"

            a.b.c sw "abc"  > a!=null && b!=null && a.b.c!=null && a.b.c sw"abc" 
            a.b.c nsw "abc" > a==null || b==null || c==null || a.b.c nsw "abc"

            a.b.c sw "abc"  > a!=null && b!=null && a.b.c!=null && a.b.c sw"abc" 
            a.b.c nsw "abc" > a==null || b==null || c==null || a.b.c nsw "abc"

            a.b.c > 5  > a!=null && a.b!=null && a.b.c > 5
            a.b.c > null > false

            a.b.c in [1,2,3] > a!=null && b!=null && c!=null && [1,2,3] contains(a.b.c)
            a.b.c in [1,2,3,null] >  a==null || a.b==null || a.b.c==null || [1,2,3] contains(a.b.c)
            a.b.c not in [1,2,3] > a==null || a.b==null || a.b.c==null || [1,2,3] not contains(a.b.c)
            a.b.c not in [1,2,3,null] > a!=null || a.b!=null || a.b.c!=null || [1,2,3] not contains(a.b.c)



            a.b.c = c.d.e

            ((a=null || a.b =null || a.b.c =null) &&(c==null || c.d==null || c.d.e==null))
                or
            (a!=null && a.b!=null && c!=null && c.d!=null && a.b.c == c.d.e)

            a.b.c != c.d.e
	

             * 
             * */

            var left = CreateFilterValueDesc(p, memberVisitor, singleItemFilter.Left);
            var right = CreateFilterValueDesc(p, memberVisitor, singleItemFilter.Right);
            return IFilterOperator.CreateOperatorExpression(left, singleItemFilter.Operator, right);
        }

        public FilterValueDesc CreateFilterValueDesc(Expression p, IMemberVisitor memberProvider, ValueInfo valueInfo)
        {
            if (valueInfo == null || valueInfo.IsConstant)
            {
                // const value
                return CreateConstValueExpression(valueInfo?.ConstantValue);
            }
            else
            {
                return CreatePathValueExpression(valueInfo.NavigatePaths);
            }
            FilterValueDesc CreateConstValueExpression(object value)
            {
                return new FilterValueDesc
                {
                    ConstValue = value
                };
            }
            FilterValueDesc CreatePathValueExpression(List<ValuePath> pathInfos)
            {

                IMemberVisitor currentMemberProvider = memberProvider;
                Type currentExpressionType = currentMemberProvider.CurrentType;
                Expression currentExpression = p;
                foreach (var pathInfo in pathInfos)
                {
                    if (pathInfo.IsFunction)
                    {
                        var functionResult = IFilterFunction.ExecuteFunction(pathInfo.Name, pathInfo.FunctionArgs, new ExecuteContext
                        {
                            CurrentExpression = currentExpression,
                            MemberVisitor = currentMemberProvider,
                            CurrentType = currentExpressionType,
                        });
                        currentExpressionType = functionResult.LambdaValueType;
                        currentExpression = currentExpression.Connect(functionResult.LambdaExpression);
                        currentMemberProvider = functionResult.MemberProvider;
                    }
                    else
                    {
                        var memberInfo = currentMemberProvider.GetSubMemberInfo(pathInfo.Name);

                        if (memberInfo == null)
                        {
                            throw Errors.InvalidMemberNameInFieldName(pathInfo.Name);
                        }
                        else
                        {
                            currentExpressionType = memberInfo.ExpressionValueType;
                            currentExpression = currentExpression.Connect(memberInfo.SelectExpression);
                            currentMemberProvider = memberInfo.SubProvider;
                        }
                    }
                }
                return new FilterValueDesc
                {
                    PathValueType = currentExpressionType,
                    PathValueExpression = currentExpression
                };
            }
        }



        class Errors
        {

            public static Exception InvalidFieldName(string name)
            {
                return new FieldInfo2ExpressionException($"Invalid field name '{name}' in filter info.");
            }
            public static Exception InvalidMemberNameInFieldName(string memberName)
            {
                return new FieldInfo2ExpressionException($"Invalid member name '{memberName}'.");
            }
            public static Exception NotSupportedFieldName(string name)
            {
                return new FieldInfo2ExpressionException($"Not supported field name '{name}' in filter info.");
            }

        }
    }
}
