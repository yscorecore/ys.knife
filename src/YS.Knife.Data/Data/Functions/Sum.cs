using System;

namespace YS.Knife.Data.Functions
{
    class Sum : CollectionFunction
    {
        protected override FunctionResult ExecuteEnumable(FunctionContext functionContext, Type itemType)
        {
            throw new NotImplementedException();
        }

        protected override FunctionResult ExecuteQueryable(FunctionContext functionContext, Type itemType)
        {
            throw new NotImplementedException();
        }
    }
}
