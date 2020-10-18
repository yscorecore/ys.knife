using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace YS.Knife.Hosting.Web.Filters
{
    public class WrapCodeMessageAttribute : Attribute, IResultFilter,IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.Result = new ObjectResult(new
            {
                Code = "error",
                Message = context.Exception.Message,
            }) ;
            context.ExceptionHandled = true;
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {

        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult obj)
            {
                context.Result = new ObjectResult(new
                {
                    Code = "0",
                    Message = "success",
                    Result = obj.Value,

                });
            }
            else if (context.Result is EmptyResult)
            {
                context.Result = new ObjectResult(new
                {
                    Code = "0",
                    Message = "success"
                });
            }
        }
    }
}
