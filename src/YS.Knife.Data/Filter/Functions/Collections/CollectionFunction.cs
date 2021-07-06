using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data.Filter.Functions.Collections
{
    public abstract class CollectionFunction : BaseFunction, IFilterFunction
    {
        private static Type[] SupportNumberTypes = new[]
        {
            typeof(decimal), typeof(long), typeof(double), typeof(float), typeof(int), typeof(decimal?),
            typeof(long?), typeof(double?), typeof(float?), typeof(int?)
        };

        public override FunctionResult Execute(FunctionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
