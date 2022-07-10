using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;

namespace YS.Knife.Rest.AspNetCore
{
    public class CodeResultApplicationModelProvider : IApplicationModelProvider
    {
        public int Order => 3;

        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
        }

        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {

            foreach (ControllerModel controller in context.Result.Controllers)
            {
                var hasAppLevelFilter = HasWrapCodeResultFilter(controller.Application.Filters);

                var hasControllerLevel = HasWrapCodeResultFilter(controller.Filters);
                foreach (ActionModel action in controller.Actions)
                {

                    var hasActionLevel = HasWrapCodeResultFilter(action.Filters);

                    if (hasAppLevelFilter || hasControllerLevel || hasActionLevel)
                    {

                        var returnType = GetRuntimeReturnType(action);
                        if (returnType == null)
                        {
                            TranslateProducesResponseType(action);
                        }
                        else if (returnType == typeof(void))
                        {
                            AddProducesResponseTypeAttribute(action, typeof(CodeResult), StatusCodes.Status200OK);
                        }
                        else
                        {
                            AddProducesResponseTypeAttribute(action, typeof(CodeResult<>).MakeGenericType(returnType), StatusCodes.Status200OK);
                        }

                    }
                }
            }
            bool HasWrapCodeResultFilter(IList<IFilterMetadata> filters)
            {
                return filters.Any(p => p is WrapCodeResultAttribute ||
                    (p is TypeFilterAttribute tf && tf.ImplementationType == typeof(WrapCodeResultAttribute)) ||
                    (p is ServiceFilterAttribute sf && sf.ServiceType == typeof(WrapCodeResultAttribute)));
            }

            Type GetRuntimeReturnType(ActionModel action)
            {
                Type returnDataType = action.ActionMethod.ReturnType;
                if (returnDataType == typeof(void) || returnDataType == typeof(Task) || returnDataType == typeof(ValueTask))
                {
                    return typeof(void);
                }
                if (returnDataType.IsGenericType && (returnDataType.GetGenericTypeDefinition() == typeof(Task<>) || returnDataType.GetGenericTypeDefinition() == typeof(ValueTask<>)))
                {
                    returnDataType = returnDataType.GetGenericArguments()[0];
                }
                if (typeof(IActionResult).IsAssignableFrom(returnDataType))
                {
                    return null;
                }
                return returnDataType;
            }
            void TranslateProducesResponseType(ActionModel action)
            {
                var attrs = action.Attributes.OfType<IApiResponseMetadataProvider>()
                    .Where(p => p.Type != null && p.Type != typeof(void) && !p.Type.IsSubclassOf(typeof(CodeResult))).ToList();
                foreach (var att in attrs)
                {
                    action.Filters.Add(new ProducesResponseTypeAttribute(typeof(CodeResult<>).MakeGenericType(att.Type), att.StatusCode));
                }
            }

        }


        public void AddProducesResponseTypeAttribute(ActionModel action, Type returnType, int statusCodeResult)
        {
            action.Filters.Add(new ProducesResponseTypeAttribute(returnType, statusCodeResult));
        }

    }
}
