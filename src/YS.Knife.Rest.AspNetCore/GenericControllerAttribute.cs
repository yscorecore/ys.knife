using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace YS.Knife.Rest.AspNetCore
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class GenericControllerAttribute : Attribute, IControllerModelConvention
    {
        public GenericControllerAttribute(Type genericControllerType)
        {
            this.GenericControllerType = genericControllerType;
        }

        public Type GenericControllerType { get; }

        public virtual void Apply(ControllerModel controller)
        {
            if (!controller.ControllerType.IsGenericType ||
                controller.ControllerType.GetGenericTypeDefinition() != GenericControllerType)
            {
                return;
            }

            string controllerName = ResolveControllerName(controller.ControllerType.GenericTypeArguments);
            controller.ControllerName = controllerName;
            controller.RouteValues["Controller"] = controllerName;
        }

        protected abstract string ResolveControllerName(Type[] genericTypes);

        protected abstract IEnumerable<Type[]> GetAllGenericTypes();

        internal void ApplyControllerFeature(IEnumerable<ApplicationPart> _, ControllerFeature feature)
        {
            foreach (var argTypes in GetAllGenericTypes())
            {
                var controllerType = GenericControllerType.MakeGenericType(argTypes);
                feature.Controllers.Add(controllerType.GetTypeInfo());
            }
        }
    }
}
