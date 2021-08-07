using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Data.Query.Expressions
{
    public class ExpressionValue
    {
        public bool IsConst { get => ValueInfo == null || ValueInfo.IsConstant; }
        public ValueInfo ValueInfo { get; set; }

        public IValueLambdaProvider ValueLambda { get; set; }
    }
}
