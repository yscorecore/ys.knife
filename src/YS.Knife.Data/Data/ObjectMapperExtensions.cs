using System;
using System.Linq.Expressions;
using YS.Knife.Data.Mappers;
using YS.Knife.Data.Translaters;

namespace YS.Knife.Data
{
    public static class ObjectMapperExtensions
    {
        public static Expression<Func<TSource, bool>> CreateFilterExpression<TSource, TTarget>(this ObjectMapper<TSource, TTarget> mapper,  FilterInfo targetFilter)
            where TSource : class
            where TTarget : class, new()
        {
            var p = Expression.Parameter(typeof(TSource), "ps");
            var res = FromContidtionInternal(typeof(TTarget), targetFilter, p,mapper);
            return Expression.Lambda<Func<TSource, bool>>(res, p);
        }
        private static Expression FromOrConditionInternal(Type targetType, FilterInfo orCondition,
            ParameterExpression p,IObjectMapper objectMapper)
        {
            if (orCondition == null) throw new ArgumentNullException(nameof(orCondition));
            Expression current = Expression.Constant(false);
            foreach (FilterInfo item in orCondition.Items.TrimNotNull())
            {
                var next = FromContidtionInternal(targetType, item, p,objectMapper);
                current = Expression.OrElse(current, next);
            }

            return current;
        }
        private static Expression FromContidtionInternal(Type targetType, FilterInfo filterInfo, ParameterExpression p,IObjectMapper objectMapper)
        {
            return filterInfo.OpType switch
            {
                OpType.AndItems => FromAndConditionInternal(targetType, filterInfo, p,objectMapper),
                OpType.OrItems => FromOrConditionInternal(targetType, filterInfo, p,objectMapper),
                _ => FromSingleItemFilterInfo(targetType, filterInfo, p,objectMapper)
            };
        }

        private static Expression FromAndConditionInternal(Type targetType, FilterInfo andCondition,
            ParameterExpression p,IObjectMapper objectMapper)
        {
            if (andCondition == null) throw new ArgumentNullException(nameof(andCondition));
            Expression current = Expression.Constant(true);
            foreach (FilterInfo item in andCondition.Items.TrimNotNull())
            {
                var next = FromContidtionInternal(targetType, item, p,objectMapper);
                current = Expression.AndAlso(current, next);
            }
            return current;
        }
        
        private static Expression FromSingleItemFilterInfo(Type targetType, FilterInfo singleItem,
            Expression p,IObjectMapper objectMapper)
        {
            if (singleItem == null) throw new ArgumentNullException(nameof(singleItem));
            var fieldPaths = FieldPath.ParsePaths(singleItem.FieldName);
            var currentTargetType = targetType;
            var currentObjectMapper = objectMapper;
            foreach (var path in fieldPaths)
            {
                if (path.IsFunction)
                {
                    throw new NotImplementedException();
                }
                else
                {
                  var fieldExpression =  currentObjectMapper.GetFieldExpression(path.Field, StringComparison.InvariantCultureIgnoreCase);
                  if (fieldExpression == null)
                  {
                      throw new Exception($"can not find property mapper for '{path.Field}'.");
                  }

                  if (fieldExpression.IsCollection)
                  {
                      throw new NotImplementedException();
                  }
                  else
                  {
                     var lambda = fieldExpression.GetLambdaExpression();
                     
                  }

                }
            }
            return null;
            
            
            // 
            // if (string.IsNullOrEmpty(singleItem.FieldName)) throw new ArgumentException("FieldName必须填充");
            // var paths = singleItem.FieldName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            // PropertyInfo pinfo;
            // Type ty = targetType;
            // Expression exp = p;
            // for (int i = 0; i < paths.Length - 1; i++)
            // {
            //     pinfo = ty.GetProperty(paths[i]);
            //     if (pinfo == null)
            //         throw new ArgumentException($"在类型{ty.FullName}中无法找到指定的属性{paths[i]}");
            //     ty = pinfo.PropertyType;
            //     exp = Expression.Property(exp, pinfo);
            // }
            //
            // pinfo = ty.GetProperty(paths.Last());
            // if (pinfo == null)
            //     throw new ArgumentException($"在类型{ty.FullName}中无法找到指定的属性{paths.Last()}");
            //
            // var converter = GetConvertByFilterType(singleItem.FilterType);
            // var val = singleItem.Value;
            // if (val == DBNull.Value) val = null; //忽略dbnull.value
            // return converter.ConvertValue(exp, pinfo, val, singleItem.Items);
        }
      
    }
}
