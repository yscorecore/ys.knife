using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace YS.Knife.Api.Client.Generator
{
    public class ControllerDescriptor
    {
        public string RouterTemplate { get; private set; }
        public string ClassName { get; private set; }
        public Type ClassInfo { get; private set; }
        public IList<ActionDescriptor> Actions { get; private set; }
        public static ControllerDescriptor FromControllerType(Type controllerType)
        {
            if (typeof(ControllerBase).IsAssignableFrom(controllerType))
            {
                throw new ArgumentException($"The type of {nameof(controllerType)} must inherits from {typeof(ControllerBase).FullName}");
            }
            var router = controllerType.GetCustomAttribute<RouteAttribute>(true);

            var actions = from p in controllerType.GetMethods(BindingFlags.CreateInstance | BindingFlags.Public)
                          where Attribute.IsDefined(p, typeof(HttpMethodAttribute), true) && !p.IsAbstract && !p.IsGenericMethod
                          select ActionDescriptor.FromActionMethod(p);
            return new ControllerDescriptor
            {
                ClassInfo = controllerType,
                ClassName = controllerType.Name,
                RouterTemplate = router?.Template,
                Actions = actions.ToList()
            };
        }
    }
}
