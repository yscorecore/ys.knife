using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using System.Linq;
using System.Collections.Concurrent;
namespace YS.Knife.Aop
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface, Inherited = false)]
    public class ParameterValidationAttribute : BaseAopAttribute
    {
        static readonly ValidationAttributeCache validationAttributeCache = new ValidationAttributeCache();
        public ParameterValidationAttribute()
        {
            this.Order = 2000;
        }
        public override Task Invoke(AspectContext context, AspectDelegate next)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            _ = next ?? throw new ArgumentNullException(nameof(next));
            var implParameters = context.ImplementationMethod.GetParameters();
            var serviceParameters = context.ServiceMethod.GetParameters();
            for (int i = 0; i < implParameters.Length; i++)
            {
                var implParameterInfo = implParameters[i];
                var serviceParameterInfo = serviceParameters[i];
                object value = context.Parameters[i];
                var validationAttributes = validationAttributeCache.GetValidationAttributes(implParameterInfo, serviceParameterInfo);
                if (value is null)
                {
                    var nullValueContext = new ValidationContext(new object(), context.ServiceProvider, null)
                    {
                        MemberName = implParameterInfo.Name
                    };
                    Validator.ValidateValue(value, nullValueContext, validationAttributes);
                }
                else
                {
                    var objectContext = new ValidationContext(value, context.ServiceProvider, null)
                    {
                        MemberName = implParameterInfo.Name
                    };
                    Validator.ValidateValue(value, objectContext, validationAttributes);
                    Validator.ValidateObject(value, objectContext, true);
                }
            }
            return next.Invoke(context);
        }
        private class ValidationAttributeCache
        {
            readonly ConcurrentDictionary<int, IEnumerable<ValidationAttribute>> cacheData = new ConcurrentDictionary<int, IEnumerable<ValidationAttribute>>();
            public IEnumerable<ValidationAttribute> GetValidationAttributes(ParameterInfo implParameterInfo, ParameterInfo serviceParameterInfo)
            {
                var key = implParameterInfo.GetHashCode() ^ serviceParameterInfo.GetHashCode();
                IEnumerable<ValidationAttribute> attributes;
                if (cacheData.TryGetValue(key, out attributes))
                {
                    return attributes;
                }
                else
                {
                    attributes = GetValidationAttributesInternal(implParameterInfo, serviceParameterInfo);
                    cacheData[key] = attributes;
                    return attributes;
                }
            }
            private List<ValidationAttribute> GetValidationAttributesInternal(ParameterInfo implParameterInfo, ParameterInfo serviceParameterInfo)
            {
                var results = new List<ValidationAttribute>(implParameterInfo.GetCustomAttributes<ValidationAttribute>(true));
                foreach (var interfaceValidationAttribute in serviceParameterInfo.GetCustomAttributes<ValidationAttribute>(true))
                {
                    if (results.Any(p => p.GetType() == interfaceValidationAttribute.GetType()))
                    {
                        continue;
                    }
                    results.Add(interfaceValidationAttribute);
                }
                return results;
            }
        }
    }
}
