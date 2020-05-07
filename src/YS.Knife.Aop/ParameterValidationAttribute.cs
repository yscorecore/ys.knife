using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using System.Linq;
namespace YS.Knife.Aop
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface, Inherited = false)]
    public class ParameterValidation : BaseAopAttribute
    {
        public ParameterValidation()
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

                var valueContext = new ValidationContext(new object(), context.ServiceProvider, null)
                {
                    MemberName = implParameterInfo.Name
                };
                var validationAttributes = GetParameterValidationAttributes(implParameterInfo, serviceParameterInfo);
                Validator.ValidateValue(value, valueContext, validationAttributes);
                var objectContext = new ValidationContext(value, context.ServiceProvider, null)
                {
                    MemberName = implParameterInfo.Name
                };
                Validator.ValidateObject(value, objectContext, true);
            }
            return next.Invoke(context);
        }


        private IEnumerable<ValidationAttribute> GetParameterValidationAttributes(ParameterInfo implParameterInfo, ParameterInfo serviceParameterInfo)
        {
            return implParameterInfo.GetCustomAttributes<ValidationAttribute>(true)
                .Concat(serviceParameterInfo.GetCustomAttributes<ValidationAttribute>(true));

        }


    }
}
