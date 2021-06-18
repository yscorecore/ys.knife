using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using YS.Knife.Data.Functions;
using YS.Knife.Data.Mappers;
using YS.Knife.Data.Translaters;

namespace YS.Knife.Data
{
    public static class ObjectMapperExtensions
    {
        public static Expression<Func<TSource, bool>> CreateSourceFilterExpression<TSource, TTarget>(this ObjectMapper<TSource, TTarget> mapper,  FilterInfo targetFilter)
            where TSource : class
            where TTarget : class, new()
        {
            var p = Expression.Parameter(typeof(TSource), "p");
            var res = FromContidtionInternal(typeof(TTarget), typeof(TSource),targetFilter, p,mapper);
            return Expression.Lambda<Func<TSource, bool>>(res, p);
        }
        private static Expression FromOrConditionInternal(Type targetType,  Type sourceType,FilterInfo orCondition,
            ParameterExpression p,IObjectMapper objectMapper)
        {
            if (orCondition == null) throw new ArgumentNullException(nameof(orCondition));
            Expression current = Expression.Constant(false);
            foreach (FilterInfo item in orCondition.Items.TrimNotNull())
            {
                var next = FromContidtionInternal(targetType, sourceType,item, p,objectMapper);
                current = Expression.OrElse(current, next);
            }

            return current;
        }
        private static Expression FromContidtionInternal(Type targetType, Type sourceType, FilterInfo filterInfo, ParameterExpression p,IObjectMapper objectMapper)
        {
            return filterInfo.OpType switch
            {
                OpType.AndItems => FromAndConditionInternal(targetType, sourceType,filterInfo, p,objectMapper),
                OpType.OrItems => FromOrConditionInternal(targetType, sourceType,filterInfo, p,objectMapper),
                _ => FromSingleItemFilterInfo(targetType, sourceType,filterInfo, p,objectMapper)
            };
        }

        private static Expression FromAndConditionInternal(Type targetType,Type sourceType, FilterInfo andCondition,
            ParameterExpression p,IObjectMapper objectMapper)
        {
            if (andCondition == null) throw new ArgumentNullException(nameof(andCondition));
            Expression current = Expression.Constant(true);
            foreach (FilterInfo item in andCondition.Items.TrimNotNull())
            {
                var next = FromContidtionInternal(targetType, sourceType,item, p,objectMapper);
                current = Expression.AndAlso(current, next);
            }
            return current;
        }
        
        private static Expression FromSingleItemFilterInfo(Type targetType,Type sourceType, FilterInfo singleItem,
            Expression p,IObjectMapper objectMapper)
        {
            if (singleItem == null) throw new ArgumentNullException(nameof(singleItem));
            var fieldPaths = FieldPath.ParsePaths(singleItem.FieldName);
            var context = new ExpressionContext()
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
           
            foreach (var path in fieldPaths)
            {
                if (path.IsFunction)
                {
                    var function = FunctionExpression.GetFunctionByName(path.FuncName);
                    if (function == null)
                    {
                        throw Errors.NotSupportFunction(path.FuncName);
                    }

                    if (!context.Current.IsCollection)
                    {
                        throw Errors.FunctionCanOnlyBeUseInCollectionType(path.FuncName);
                    }

                    FunctionContext functionContext = new FunctionContext
                    {
                         SourceType = context.Current.SourceValueType,
                         TargetType = context.Current.TargetValueType,
                         SubPaths = path.SubPaths,
                         SubTypeMapper = context.Current.ObjectMapper
                    };

                    var functionResult = function.Execute(functionContext);
                    
                    context.Add(new PathSegment
                    {
                        IsCollection = false,
                        SourceExpression = functionResult.Expression,
                        TargetValueType = functionResult.TargetValueType,
                        SourceValueType =functionResult.SourceValueType,
                        ObjectMapper = functionResult.ObjectMapper
                    });

                }
                else
                { 
                      var fieldExpression = context.GetField(path.Field);
                      if (fieldExpression == null)
                      {
                          throw Errors.MissMemberException(singleItem.FieldName, path.Field);
                      }
                      context.Add(new PathSegment
                      {
                          IsCollection = fieldExpression.IsCollection,
                          TargetValueType = fieldExpression.TargetValueType,
                          SourceValueType = fieldExpression.SourceValueType,
                          ObjectMapper = fieldExpression.SubMapper,
                          SourceExpression = fieldExpression.SourceExpression
                      });
                }
            }
            return CompareWithValue(context,singleItem);

        }

        private static Expression CompareWithValue(ExpressionContext context, FilterInfo singleItem)
        {
            
            var current = context.Current;
            if (current.IsCollection)
            {
                throw new Exception("....");
            }

            if (singleItem.Function != null)
            {
                
            }


            var right = Expression.Constant(singleItem.Value);

            return  Expression.Equal(right, right);
        }
        //private static Expression

        class ExpressionContext:List<PathSegment>
        {

            public PathSegment Current{ get=> this.Last(); }
            
            public IMapperExpression GetField(string targetProp)
            {
                var current = this.Current;
                if (current.ObjectMapper != null)
                {
                    return current.ObjectMapper.GetFieldExpression(targetProp, StringComparison.InvariantCultureIgnoreCase);
                }

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

    static class Errors
    {
        public static Exception MissMemberException(string fullField, string fieldPath)
        {
            return new Exception($"can not find property '{fieldPath}' in field path '{fullField}'.");;
        }

        public static Exception NotSupportFunction(string functionName)
        {
            return new Exception($"the function '{functionName}' not supported.");;
        }
        public static Exception FunctionCanOnlyBeUseInCollectionType(string functionName)
        {
            return new Exception($"the function '{functionName}' only can be use in collection type.");;
        }
    }
}
