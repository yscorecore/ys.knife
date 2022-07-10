using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace YS.Knife.Rest.AspNetCore
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class WrapCodeResultAttribute : Attribute, IExceptionFilter, IResultFilter
    {
        public string SuccessMessage { get; set; } = "success";
        public string SuccessCode { get; set; } = "0";
        public string DefaultExceptionCode { get; set; } = StatusCodes.Status500InternalServerError.ToString();
        public int DefaultExceptionHttpStatusCode { get; set; } = StatusCodes.Status500InternalServerError;
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is CodeException codeException)
            {
                context.Result = new ObjectResult(
                    CodeResult.FromCodeException(codeException));
            }
            else
            {
                context.Result = new ObjectResult(
                     CodeResult.FromCode(DefaultExceptionCode, context.Exception.Message, context.Exception.Data))
                {
                    StatusCode = DefaultExceptionHttpStatusCode
                };
                context.ExceptionHandled = true;
            }
            context.ExceptionHandled = true;
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {

        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult obj && obj.Value is not CodeResult)
            {
                if (IsSuccessCode(obj))
                {
                    context.Result = new ObjectResult(CodeResult.FromData(SuccessCode, SuccessMessage, obj.Value)) { StatusCode = obj.StatusCode };
                }
                else
                {
                    context.Result = new ObjectResult(CodeResult.FromData($"{obj.StatusCode}", obj.Value?.ToString(), obj.Value)) { StatusCode = obj.StatusCode };
                }
            }
            else if (context.Result is EmptyResult)
            {
                context.Result = new ObjectResult(CodeResult.FromCode(SuccessCode, SuccessMessage));
            }

            static bool IsSuccessCode(ObjectResult obj)
            {
                return obj.StatusCode == null || (obj.StatusCode >= 200 && obj.StatusCode < 300);
            }
        }
    }
}
