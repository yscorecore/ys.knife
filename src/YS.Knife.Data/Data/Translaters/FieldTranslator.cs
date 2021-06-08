using System;
using System.Collections.Generic;
using System.Linq;
using YS.Knife.Data.Mappers;

namespace YS.Knife.Data.Translaters
{
    public static class FieldTranslator
    {
        public static string Translate<TFrom,TTo>(string fieldExpression, ObjectMapper<TFrom,TTo> mapper)
            where TFrom:class
            where TTo:class,new()
        {
            _ = fieldExpression ?? throw new ArgumentNullException(nameof(fieldExpression));
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
            var fieldPaths = FieldPath.ParsePaths(fieldExpression);
            return FieldPath.JoinPaths(Translate(fieldPaths,mapper));
        }

        private static List<FieldPath> Translate(List<FieldPath> fieldPaths, Type sourceType, Type targetType)
        {
            if (fieldPaths == null) return null;
            return null;
        }

        private static List<FieldPath> Translate(List<FieldPath> fieldPaths,  IObjectMapper mapper)
        {
            if (fieldPaths == null) return null;
            List<FieldPath> results = new List<FieldPath>();
            foreach (var fieldPath in fieldPaths)
            {
                var mapExpression = mapper.GetFieldExpression(fieldPath.Field, StringComparison.CurrentCultureIgnoreCase);
                if (fieldPath.IsFunction)
                {
                    if (!mapExpression.IsCollection)
                    {
                        throw new Exception("function only support collection property.");
                    }

                    if (mapExpression.SubMapper != null)
                    {
                          results.Add(new FieldPath
                          {
                               FuncName = fieldPath.FuncName,
                               SubPaths = Translate(fieldPath.SubPaths,mapExpression.SubMapper)
                          }); 
                    }
                    else
                    {
                        var sourceItemType = EnumerableTypeUtils.GetEnumerableItemType(mapExpression.SourceValueType);
                        // targetItemType
                        results.Add( new FieldPath
                        {
                            FuncName = fieldPath.FuncName,
                            SubPaths = Translate(fieldPath.SubPaths,sourceItemType,sourceItemType)
                        });
                    }
                }
                else
                {
                    
                    // only field
                }

            }
            return results;
        }

        public static FilterInfo Translate<TFrom, TTo>(this FilterInfo filterInfo, ObjectMapper<TFrom, TTo> mapper)
            where TFrom:class
            where TTo:class,new()
        {
            if (filterInfo == null) return null;
            return new FilterInfo
            {
                FieldName = Translate(filterInfo.FieldName,mapper),
                OpType = filterInfo.OpType,
                FilterType = filterInfo.FilterType,
                Value = filterInfo.Value,
                Items = filterInfo.Items?.Select(p=>Translate(p,mapper)).ToList()
            };
        }
        public static OrderInfo Translate<TFrom, TTo>(this OrderInfo orderInfo, ObjectMapper<TFrom, TTo> mapper)
            where TFrom:class
            where TTo:class,new()
        {
            if (orderInfo?.Items != null)
            {
                return new OrderInfo(orderInfo.Items.Select(p => p.Translate(mapper)));
            }
            return orderInfo;
        }
        public static OrderItem Translate<TFrom, TTo>(this OrderItem orderItem, ObjectMapper<TFrom, TTo> mapper)
            where TFrom:class
            where TTo:class,new()
        {
            return orderItem!=null? new OrderItem(Translate(orderItem.FieldName,mapper),orderItem.OrderType): null;
        }
        
        public static QueryInfo Translate<TFrom, TTo>(this QueryInfo queryInfo, ObjectMapper<TFrom, TTo> mapper)
            where TFrom:class
            where TTo:class,new()
        {
            if (queryInfo == null) return null;
            return new QueryInfo
            {
                 Filter = queryInfo.Filter.Translate(mapper),
                 Order = queryInfo.Order.Translate(mapper),
                 Limit = queryInfo.Limit,
                 Select = queryInfo.Select
            };
        }
    }
}
