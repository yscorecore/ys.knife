using System;
using System.Linq.Expressions;

namespace YS.Knife.Data.Query.Expressions
{
    public interface IFilterMemberInfo
    {

        public Type ExpressionValueType { get; }

        public LambdaExpression SelectExpression { get; }

        public IMemberVisitor SubProvider { get; }


    }
}
