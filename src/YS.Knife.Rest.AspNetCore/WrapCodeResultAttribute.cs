using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using YS.Knife.Rest.Api;

namespace YS.Knife.Rest.AspNetCore
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class WrapCodeResultAttribute : Attribute, IExceptionFilter, IResultFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is CodeException codeException)
            {
                context.Result = new ObjectResult(
                    CodeResult.FromData(codeException.Code,codeException.Message,codeException.Data));
            }
            else
            {
                context.Result = new ObjectResult(
                     CodeResult.FromData("error", context.Exception.Message, context.Exception.Data));
            }
            context.ExceptionHandled = true;
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
          
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult obj)
            {
                context.Result = new ObjectResult(CodeResult.FromData("0", "success", obj.Value));
            }
            else if (context.Result is EmptyResult)
            {
                context.Result = new ObjectResult(CodeResult.FromCode("0", "success"));
               
            }
        }
    }
}
