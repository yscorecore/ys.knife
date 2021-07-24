using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YS.Knife.Data.Query.Functions;

namespace YS.Knife.EntityFrameworkCore.Functions
{
    public class Random : YS.Knife.Data.Query.Functions.EmptyArgumentFunction
    {
        // select * from users orderby(p=>EF.Functions.Random())
        // ef6 support random function
        protected override FunctionResult OnExecute(ExecuteContext context)
        {
            //  Microsoft.EntityFrameworkCore.EF.Functions.Random
            throw new NotImplementedException();
        }
    }
}
