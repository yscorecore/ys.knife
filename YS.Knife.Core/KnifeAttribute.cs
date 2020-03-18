using System;

namespace YS.Knife
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public abstract class KnifeAttribute : Attribute
    {
        public abstract void RegisteService(KnifeServiceContext registerContext);
    }
}