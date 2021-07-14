using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static YS.Knife.Data.Filter.FilterInfoExpressionBuilder;

namespace YS.Knife.Data.Filter.Functions
{
    public class ExecuteContext
    {
        public Expression CurrentExpression { get; set; }
        public IMemberExpressionProvider MemberExpressionProvider { get; set; }

        public virtual Type CurrentType { get; set; }
    }
}
