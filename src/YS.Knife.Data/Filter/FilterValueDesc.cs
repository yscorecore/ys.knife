using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace YS.Knife.Data.Filter
{
    public class FilterValueDesc
    {
        public object Value { get; set; }

        public Type ExpressionValueType { get; set; }

        public Expression CurrentExpression { get; set; }

        public bool IsConstValue { get => CurrentExpression != null; } 
    }
}
