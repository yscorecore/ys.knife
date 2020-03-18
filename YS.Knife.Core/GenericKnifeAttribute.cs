using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife
{
    public abstract class GenericKnifeAttribute : Attribute
    {
        public void RegisteService(KnifeRegisteContext registerContext)
        {
            var proxy = OnGetGenericProxy(registerContext);
            proxy.RegisteService(registerContext);
        }
        protected abstract IGenericRegisteProxy OnGetGenericProxy(KnifeRegisteContext context);

        protected interface IGenericRegisteProxy
        {
            void RegisteService(KnifeRegisteContext registerContext);
        }
    }
}
