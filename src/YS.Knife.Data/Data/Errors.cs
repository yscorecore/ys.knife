using System;

namespace YS.Knife.Data
{
    public class Errors
    {
        public static Exception MissingFilterFieldName()
        {
            return new Exception("Miss field name in filter info.");
        }

        public static Exception MissingOrderByFieldName()
        {
            return new Exception("Miss order by field name");
        }

        public static Exception InvalidFieldName(string fieldName)
        {
            return new Exception($"Invalid field name '{fieldName}'.");
        }

        public static Exception CanNotUseNavigationMemberInCollectionType(string fieldPart,string fieldName)
        {
            return new Exception($"Invalid field name '{fieldName}', can not use navigation member '{fieldPart}' in collection type.");
        }

        public static Exception OnlyCanUseFunctionInCollectionType()
        {
            return new Exception("only can use function in collection type");
        }

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
