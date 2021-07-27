using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace YS.Knife.Rest.AspNetCore
{
    public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var attributes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(p => p.GetTypes())
                .Where(p => !p.IsAbstract && p.IsGenericType && typeof(ControllerBase).IsAssignableFrom(p) &&
                            p.IsDefined(typeof(GenericControllerAttribute), true))
                .Select(p => p.GetCustomAttribute<GenericControllerAttribute>());
            foreach (var genericAttribute in attributes)
            {
                genericAttribute.ApplyControllerFeature(parts, feature);
            }
        }
    }
}
