using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace System.AspnetCore
{
    public class QueryModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (context.Metadata.ModelType == typeof(SearchCondition))
            {
                return new BinderTypeModelBinder(typeof(SearchConditionBinder));
            }
            return null;
        }
    }
}
