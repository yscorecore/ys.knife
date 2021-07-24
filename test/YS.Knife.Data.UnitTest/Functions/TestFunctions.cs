using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Data.Query.Functions;

namespace YS.Knife.Data.Query.Functions
{
   
    public  class TestFunctions
    {
        class Func : ValueInfoFunction
        {
            protected override int MinArgLength { get => 0; }
            protected override int MaxArgLength { get => 10; }

            protected override FunctionResult OnExecute(List<ValueInfo> args, ExecuteContext context)
            {
                throw new NotImplementedException();
            }


        }

        class A : EmptyArgumentFunction
        {
            protected override FunctionResult OnExecute(ExecuteContext context)
            {
                throw new NotImplementedException();
            }
        }
        class B : EmptyArgumentFunction
        {
            protected override FunctionResult OnExecute(ExecuteContext context)
            {
                throw new NotImplementedException();
            }
        }
        class C : EmptyArgumentFunction
        {
            protected override FunctionResult OnExecute(ExecuteContext context)
            {
                throw new NotImplementedException();
            }
        }
        class D : EmptyArgumentFunction
        {
            protected override FunctionResult OnExecute(ExecuteContext context)
            {
                throw new NotImplementedException();
            }
        }
        class E : EmptyArgumentFunction
        {
            protected override FunctionResult OnExecute(ExecuteContext context)
            {
                throw new NotImplementedException();
            }
        }
        class Add2 : ValueInfoFunction
        {
            protected override int MinArgLength { get => 1; }
            protected override int MaxArgLength { get => 1; }

            protected override FunctionResult OnExecute(List<ValueInfo> args, ExecuteContext context)
            {
                throw new NotImplementedException();
            }


        }
    }
}
