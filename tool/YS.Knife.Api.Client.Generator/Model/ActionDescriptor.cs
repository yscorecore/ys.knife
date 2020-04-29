using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace YS.Knife.Api.Client.Generator.Model
{
    public class ActionDescriptor
    {
        public string RouterTemplate { get; private set; }
        public string MethodName { get; private set; }
        public HttpMethod HttpMethod { get; private set; }
        public MethodInfo MethodInfo { get; private set; }

        public IList<ArgumentDescriptor> Arguments { get; private set; }

        public static ActionDescriptor FromActionMethod(MethodInfo methodInfo)
        {
            var methodAttribute = methodInfo.GetCustomAttribute<HttpMethodAttribute>(true) ?? throw new ArgumentException($"The method must define a {nameof(HttpMethodAttribute)}.");

            string httpMethodName = methodAttribute.Name.Replace("Http", string.Empty).Replace("Attribute", string.Empty);

            var router = methodInfo.GetCustomAttribute<RouteAttribute>(true);

            var arguments = from p in methodInfo.GetParameters()
                            select ArgumentDescriptor.FromParameter(p);

            return new ActionDescriptor
            {
                MethodInfo = methodInfo,
                MethodName = methodInfo.Name,
                HttpMethod = httpMethodName,
                RouterTemplate = string.IsNullOrEmpty(methodAttribute.Template) ? router?.Template : methodAttribute.Template,
                Arguments = arguments.ToList()
            };
        }

    }
}
