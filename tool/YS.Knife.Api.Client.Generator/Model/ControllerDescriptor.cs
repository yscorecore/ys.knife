using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace YS.Knife.Api.Client.Generator.Model
{
    public class ControllerDescriptor
    {
        public string RouterTemplate { get; private set; }
        public string ClassName { get; private set; }
        public string ControllerName
        {
            get
            {
                if (!string.IsNullOrEmpty(ClassName) && ClassName.EndsWith("Controller", StringComparison.InvariantCultureIgnoreCase))
                {
                    return ClassName.Substring(0, ClassName.Length - "Controller".Length);
                }
                return string.Empty;
            }
        }
        public IList<ActionDescriptor> Actions { get; private set; }
        public Type InterfaceType { get; private set; }
        public static ControllerDescriptor FromControllerType(Type controllerType)
        {
            _ = controllerType ?? throw new ArgumentNullException(nameof(controllerType));
            if (!typeof(ControllerBase).IsAssignableFrom(controllerType))
            {
                throw new ArgumentException($"The type of {nameof(controllerType)} must inherits from {typeof(ControllerBase).FullName}");
            }
            return new ControllerDescriptor
            {
                ClassName = controllerType.Name,
                RouterTemplate = controllerType.GetCustomAttribute<RouteAttribute>(true)?.Template,
                InterfaceType = controllerType.GetInterfaces().FirstOrDefault(),
                Actions = FindActionMethods(controllerType).Select(ActionDescriptor.FromActionMethod).ToList()
            };
        }
        private static IEnumerable<MethodInfo> FindActionMethods(Type type)
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (Attribute.IsDefined(method, typeof(HttpMethodAttribute), true)
                    && !method.IsAbstract && !method.IsGenericMethod)
                {
                    yield return method;
                }
            }
        }
    }
}
