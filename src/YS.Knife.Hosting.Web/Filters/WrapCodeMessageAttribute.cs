using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace YS.Knife.Hosting.Web.Filters
{
    public class WrapCodeMessageAttribute : Attribute,  IExceptionFilter
    {
        public WrapCodeMessageAttribute(KnifeWebOptions knifeWebOptions)
        {
            KnifeWebOptions = knifeWebOptions;
        }

        public KnifeWebOptions KnifeWebOptions { get; }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is CodeException codeException)
            {
                context.Result = new ObjectResult(new
                {
                    Code = codeException.Code,
                    Message = context.Exception.Message,
                    Data = codeException.Data,
                });
            }
            else
            {
                context.Result = new ObjectResult(new
                {
                    Code = "error",
                    Message = context.Exception.Message,
                });
            }
            context.ExceptionHandled = true;
        }

      
    }
   
}
