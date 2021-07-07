using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data.Filter.Functions
{
    static class FunctionErrors
    {
        public static Exception NotSupportFunction(string functionName)
        {
            return new FieldInfo2ExpressionException($"Not support function '{functionName}'."); ;
        }
        public static Exception ArgumentCountNotMatched(string functionName)
        {
            return new FieldInfo2ExpressionException($" Argument count not matched '{functionName}'."); ;
        }
        public static FieldInfo2ExpressionException OnlyCanUseFunctionInCollectionType(string functionName)
        {
            return new FieldInfo2ExpressionException($"Only can use function {functionName} in collection type");
        }
    }
}
