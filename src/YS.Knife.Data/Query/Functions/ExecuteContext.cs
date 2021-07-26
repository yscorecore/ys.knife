using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using YS.Knife.Data.Filter;
using YS.Knife.Data.Query.Expressions;
using static YS.Knife.Data.Filter.FilterInfoExpressionBuilder;

namespace YS.Knife.Data.Query.Functions
{
    public class ExecuteContext
    {
        public Expression CurrentExpression { get; set; }
        public IMemberVisitor MemberVisitor { get; set; }

        public virtual Type CurrentType { get; set; }
    }
}
