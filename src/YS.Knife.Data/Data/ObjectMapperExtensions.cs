using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using YS.Knife.Data.Functions;
using YS.Knife.Data.Mappers;
using YS.Knife.Data.Translaters;

namespace YS.Knife.Data
{
    public static class ObjectMapperExtensions
    {
        public static Expression<Func<TSource, bool>> CreateSourceFilterExpression<TSource, TTarget>(
            this ObjectMapper<TSource, TTarget> mapper, FilterInfo targetFilter)
            where TSource : class
            where TTarget : class, new()
        {
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));

            var p = Expression.Parameter(typeof(TSource), "p");
            if (targetFilter == null)
            {
                return Expression.Lambda<Func<TSource, bool>>(Expression.Constant(true), p);
            }

            var res = FromContidtionInternal(typeof(TTarget), typeof(TSource), targetFilter, p, mapper);
            return Expression.Lambda<Func<TSource, bool>>(res, p);
        }

        private static Expression FromOrConditionInternal(Type targetType, Type sourceType, FilterInfo orCondition,
            ParameterExpression p, IObjectMapper objectMapper)
        {
            if (orCondition == null) throw new ArgumentNullException(nameof(orCondition));
            Expression current = Expression.Constant(false);
            foreach (FilterInfo item in orCondition.Items.TrimNotNull())
            {
                var next = FromContidtionInternal(targetType, sourceType, item, p, objectMapper);
                current = Expression.OrElse(current, next);
            }

            return current;
        }

        private static Expression FromContidtionInternal(Type targetType, Type sourceType, FilterInfo filterInfo,
            ParameterExpression p, IObjectMapper objectMapper)
        {
            return filterInfo.OpType switch
            {
                OpType.AndItems => FromAndConditionInternal(targetType, sourceType, filterInfo, p, objectMapper),
                OpType.OrItems => FromOrConditionInternal(targetType, sourceType, filterInfo, p, objectMapper),
                _ => FromSingleItemFilterInfo(targetType, sourceType, filterInfo, p, objectMapper)
            };
        }

        private static Expression FromAndConditionInternal(Type targetType, Type sourceType, FilterInfo andCondition,
            ParameterExpression p, IObjectMapper objectMapper)
        {
            if (andCondition == null) throw new ArgumentNullException(nameof(andCondition));
            Expression current = Expression.Constant(true);
            foreach (FilterInfo item in andCondition.Items.TrimNotNull())
            {
                var next = FromContidtionInternal(targetType, sourceType, item, p, objectMapper);
                current = Expression.AndAlso(current, next);
            }

            return current;
        }

        private static Expression FromSingleItemFilterInfo(Type targetType, Type sourceType, FilterInfo singleItem,
            Expression p, IObjectMapper objectMapper)
        {
            if (singleItem == null) throw new ArgumentNullException(nameof(singleItem));
            if (string.IsNullOrWhiteSpace(singleItem.FieldName)) { throw Errors.MissingFilterFieldName(); }

            var context = new FilterExpressionContext()
            {
                new PathSegment()
                {
                    SourceExpression = p,
                    SourceValueType = sourceType,
                    TargetValueType = targetType,
                    ObjectMapper = objectMapper,
                    IsCollection = false
                }
            };
            foreach (var field in FieldNameParser.Parse(singleItem.FieldName))
            {
                if (context.Current.IsCollection)
                {
                    throw Errors.CanNotUseNavigationMemberInCollectionType(field, singleItem.FieldName);
                }

                var fieldExpression = context.GetField(field);
                if (fieldExpression == null)
                {
                    throw Errors.MissMemberException(singleItem.FieldName, field);
                }

                context.Add(new PathSegment
                {
                    IsCollection = fieldExpression.IsCollection,
                    TargetValueType = fieldExpression.TargetValueType,
                    SourceValueType = fieldExpression.SourceValueType,
                    ObjectMapper = fieldExpression.SubMapper,
                    SourceExpression = context.Current.SourceExpression.Connect(fieldExpression.SourceExpression)
                });
            }

            return CompareWithValue(context, singleItem);
        }

        private static Expression CompareWithValue(FilterExpressionContext context, FilterInfo singleItem)
        {
            if (singleItem.Function != null && !string.IsNullOrEmpty(singleItem.Function.Name))
            {
                // collection
                if (!context.Current.IsCollection)
                {
                    throw Errors.OnlyCanUseFunctionInCollectionType();
                }

                return CompareFilterWithFunctionValue(context, singleItem.Function, singleItem.FilterType,
                    singleItem.Value);
            }
            else
            {
                return CompareFilterWithValue(context, singleItem.FilterType, singleItem.Value);
            }
        }

        private static Expression CompareFilterWithFunctionValue(FilterExpressionContext context,
            FunctionInfo functionInfo, FilterType filterType,
            object value)
        {
            var function = FunctionExpression.GetFunctionByName(functionInfo.Name);
            if (function == null)
            {
                throw Errors.NotSupportFunction(functionInfo.Name);
            }

            return null;
        }

        private static Expression CompareFilterWithValue(FilterExpressionContext context, FilterType filterType,
            object value)
        {
          return  Expression.Equal(context.Current.SourceExpression, Expression.Constant(value));
        }


        //private static Expression

        class FilterExpressionContext : List<PathSegment>
        {
            public PathSegment Current { get => this.Last(); }

            public IMapperExpression GetField(string targetProp)
            {
                var current = this.Current;
                if (current.ObjectMapper != null)
                {
                    return current.ObjectMapper.GetFieldExpression(targetProp,
                        StringComparison.InvariantCultureIgnoreCase);
                }

                // TODO
                return null;
            }
        }

        class PathSegment
        {
            public Type SourceValueType { get; set; }

            public Type TargetValueType { get; set; }

            public IObjectMapper ObjectMapper { get; set; }

            public bool IsCollection { get; set; }

            public Expression SourceExpression { get; set; }
        }
    }
}
