using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Principal;
using YS.Knife.Data.Functions;

namespace YS.Knife.Data.Translaters
{
    public class FilterTranslater
    {
        public FilterInfo Translate<TFrom, TTo>(FilterInfo filterInfo)
        {
            return Translate(typeof(TFrom), typeof(TTo), filterInfo);
        }
        public FilterInfo Translate(Type from, Type to, FilterInfo filterInfo)
        {
            if (filterInfo == null) return null;
            return new FilterInfo
            {
                FieldName = TranslateFieldName(from,to,filterInfo.FieldName),
                OpType = filterInfo.OpType,
                FilterType = filterInfo.FilterType,
                Items = filterInfo.Items?.Select(p=>Translate(@from,to,p)).ToList()
            };
        }
        private string TranslateFieldName(Type from, Type to, string field)
        {
            if (string.IsNullOrEmpty(field)) return field;
            var paths = FieldPath.ParsePaths(field);
            _ = ValidateFieldPaths(paths, from);
            return null;
        }

        private Type ValidateFieldPaths(List<FieldPath> paths, Type from)
        {
            var currentType = from;
            foreach (var path in paths ?? Enumerable.Empty<FieldPath>() )
            {
                if (path.IsFunction)
                {
                    ValidateFunctionName(path.FuncName);
                    ValidateType(currentType);
                    var lastType = ValidateFieldPaths(path.SubPaths, currentType.GetEnumerableSubType());
                    currentType = GetFunctionReturnType(path.Field, lastType);
                }
                else
                {
                   var propMap =  currentType.GetProperties().ToDictionary(p => p.Name, p=>p,StringComparer.InvariantCultureIgnoreCase);
                   if (propMap.TryGetValue(path.Field,out var prop))
                   {
                       currentType = prop.PropertyType;
                   }
                   else
                   {
                       throw new FieldExpressionException($"Invalid field path segment '{path.Field}', can not find prop '{path.Field}' in type '{currentType.FullName}'.");
                   }
                }
            }
            return currentType;
        }

        static void ValidateFunctionName(string func)
        {
            if (FunctionExpression.GetFunctionByName(func) == null)
            {
                throw new FieldExpressionException($"The function name '{func}' not supported.");
            }
        }

        static void ValidateType(Type type)
        {
            if (!type.IsGenericEnumerable())
            {
                throw new FieldExpressionException($"Function field should be under enumerable type.");
            }
        }


        static Type GetFunctionReturnType(string func, Type currentType)
        {
            return null;
        }

    }
}
