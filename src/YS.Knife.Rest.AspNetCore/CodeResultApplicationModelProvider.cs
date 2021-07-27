using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using YS.Knife.Rest.Api;

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
                if (!controller.Attributes.OfType<WrapCodeResultAttribute>().Any())
                {
                    continue;
                }
                foreach (ActionModel action in controller.Actions)
                {
                    if (!action.Attributes.OfType<WrapCodeResultAttribute>().Any())
                    {
                        continue;
                    }

                    var (actionResult, returnType) = GetReturnType(action);

                    if (!actionResult)
                    {
                        if (returnType == null)
                        {
                            AddProducesResponseTypeAttribute(action, typeof(CodeResult), 200);
                        }
                        else
                        {
                            AddProducesResponseTypeAttribute(action, typeof(CodeResult<>).MakeGenericType(returnType), 200);
                        }
                    }

                }
            }
        }
        private (bool, Type) GetReturnType(ActionModel action)
        {
            Type returnDataType = action.ActionMethod.ReturnType;
            if (returnDataType == null || returnDataType == typeof(void) || returnDataType == typeof(Task) || returnDataType == typeof(ValueTask))
            {
                return (false, null);
            }
            if (returnDataType.IsGenericType && (returnDataType.GetGenericTypeDefinition() == typeof(Task<>) || returnDataType.GetGenericTypeDefinition() == typeof(ValueTask<>)))
            {
                returnDataType = returnDataType.GetGenericArguments()[0];
            }
            if (typeof(IActionResult).IsAssignableFrom(returnDataType))
            {
                return (true, returnDataType);
            }
            return (false, returnDataType);
        }

        public void AddProducesResponseTypeAttribute(ActionModel action, Type returnType, int statusCodeResult)
        {
            action.Filters.Add(new ProducesResponseTypeAttribute(returnType, statusCodeResult));
        }

        public void AddUniversalStatusCodes(ActionModel action, Type returnType)
        {
            AddProducesResponseTypeAttribute(action, returnType, 200);
            AddProducesResponseTypeAttribute(action, null, 500);
        }


    }
}
